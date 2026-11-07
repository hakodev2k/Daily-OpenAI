# Evidence pack — Canary release 2026-10-21 (mô phỏng)

Đây là dữ liệu giả lập phục vụ investigation; không phải production logs thật.

| Giờ | Hoạt động | App pods | Schema |
|---|---|---|---|
| 09:00 | Baseline healthy | 4 × V1.0 | S0 |
| 09:04 | Migration job báo thành công | 4 × V1.0 | S2 |
| 09:06 | Canary 1 pod V2 | 3 × V1.0 + 1 × V2 | S2 |
| 09:08 | HTTP 5xx tăng lên 7.9% | 3 × V1.0 + 1 × V2 | S2 |
| 09:11 | Rollback app V2 → V1.0 | 4 × V1.0 | S2 |
| 09:13 | 5xx vẫn cao; incident escalated | 4 × V1.0 | S2 |

Schema diff do pipeline đính kèm:

```sql
-- S0
CREATE TABLE dbo.PartnerContact (
    Id bigint NOT NULL PRIMARY KEY,
    TenantId uniqueidentifier NOT NULL,
    ContactName nvarchar(200) NOT NULL,
    RowVersion rowversion NOT NULL
);

-- migration đã áp dụng trong incident (S2)
EXEC sp_rename 'dbo.PartnerContact.ContactName', 'DisplayName', 'COLUMN';
```

Các dòng log đã ẩn thông tin người dùng:

```text
09:06:01 pod=api-v1-3 route=PUT /contacts/{id} status=500 sql=207
  SqlException: Invalid column name 'ContactName'.
09:06:07 pod=api-v2-1 route=GET /contacts/{id} status=200
09:08:22 pod=api-v1-2 route=GET /contacts/{id} status=500 sql=207
09:12:05 pod=api-v1-1 route=PUT /contacts/{id} status=500 sql=207
```

Mẫu metrics (tỷ lệ giả lập):

| Cohort | GET 5xx | PUT 5xx | CPU | DB connections |
|---|---:|---:|---:|---:|
| V1.0 pods | 8–12% | 9–13% | 31% | ổn định |
| V2 pod | <0.2% | <0.2% | 34% | ổn định |

Evidence còn thiếu: row-level write audit, số concurrent updates, lock-wait trend trong backfill, rollback artifact version, số consumers legacy. Bạn phải ghi rõ cần thu thập gì trước production approval.

## Thử nghiệm suy luận

Giả sử một row có `ContactName='Lan'`, đã backfill `DisplayName='Lan'`. Trong lúc coexistence, V1.0 đổi tên thành `'Minh'` nhưng chỉ ghi `ContactName`. Nếu V2 ưu tiên đọc `DisplayName`, V2 sẽ nhìn thấy gì? Điều này ảnh hưởng thế nào tới quyết định thời điểm chuyển read path?
