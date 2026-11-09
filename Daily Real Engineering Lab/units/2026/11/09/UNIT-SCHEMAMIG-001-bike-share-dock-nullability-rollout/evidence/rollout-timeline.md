# Evidence — staged rollout (synthetic, deterministic)

**Schema T0**
```sql
CREATE TABLE dock_events (
  id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  station_id text NOT NULL,
  occurred_at timestamptz NOT NULL,
  station_zone text NULL
);
```
Dữ liệu: 8,000,000 row; 2,100,000 row có `station_zone IS NULL`. Dữ liệu cũ có một số `station_id` đã ngừng hoạt động; bảng `stations` chỉ chứa trạm hiện hành.

**Writer contracts**
- API v1: `INSERT INTO dock_events(station_id,occurred_at) VALUES (...)`.
- API v2: ghi thêm `station_zone` từ danh mục trạm.
- Offline worker v1: có thể đồng bộ dữ liệu sau 48 giờ, không biết `station_zone`.
- Dashboard v2: chỉ cho phép hiển thị zone khi giá trị được xác thực; `UNKNOWN` không được xem như zone thật.

**Timeline dự kiến**
| Thời điểm | Sự kiện | Quan sát |
|---|---|---|
| T0 | 40% API v2, 60% API v1, worker v1 đang chạy | 2.1M NULL |
| T1 | Migration thử `ALTER TABLE dock_events ALTER COLUMN station_zone SET NOT NULL` | lỗi vì row cũ NULL |
| T2 | Nhóm thử backfill từ `stations` | 190k row không map được do trạm cũ |
| T3 | Nhóm thử gán `DEFAULT 'UNKNOWN'` rồi SET NOT NULL | insert v1 không lỗi, nhưng dashboard báo zone không hợp lệ |
| T4 | Cắt mặc định và chuyển 100% API v2, worker v1 vẫn có backlog | insert worker v1 lỗi |
| T5 | Thử rollback API v1 sau khi constraint được áp dụng | API v1 không ghi được |

**Tín hiệu bổ sung**: CPU 38%, p95 API 120ms, disk còn 45%; đây chưa phải bằng chứng về lock hay replication lag.

**Giới hạn kinh doanh**: downtime ghi = 0; mất sự kiện = 0; dữ liệu không xác minh được phải quarantine; đội vận hành có 2 người; cửa sổ rollout 14 ngày; worker offline tối đa 48 giờ. Số liệu là fixture cho tabletop, không phải đo production.