# Reference Solution — đọc sau khi tự lập rollout plan

**Một phương án có thể bảo vệ được**, không phải đáp án duy nhất. Giả định SQL Server 2022, 4 pods, mixed versions, 40 writes/s, 24h rollback (sau đó đổi thành 72h).

## 1. Symptoms

V1.0 pods báo `SqlException 207` khi truy vấn `ContactName`, trong khi V2 vẫn phục vụ. App rollback không sửa schema nên lỗi vẫn kéo dài. Tỷ lệ lỗi theo pod version đáng tin hơn tổng error rate.

## 2. Evidence

Schema diff cho thấy rename `ContactName` → `DisplayName` trước khi V1.0 rời traffic. SQL 207 trên V1.0 là bằng chứng trực tiếp của contract mismatch; CPU, DB connection bình thường là noise, không bác bỏ hypothesis. Cần xác nhận thêm deployment order, consumers, audit và write correctness.

## 3. Root cause

**Destructive schema change đi trước compatibility rollout**. Rename là thay đổi contract vật lý, không phải alias vô hại: V1.0 vẫn compile/execute SQL dùng tên cột cũ. Rollback application không rollback schema. Một lỗi thứ hai có thể xuất hiện nếu chỉ add cột + backfill: writer cũ cập nhật `ContactName` sau backfill trong khi reader mới lấy `DisplayName` cũ.

## 4. Decision: expand → compatibility → migrate → cutover → contract

**Phase A — baseline và expand.** Ghi lại cohort metrics, query dependencies, rollback artifacts. Chạy DDL additive qua pipeline có DBA review, đặt `LOCK_TIMEOUT` phù hợp và lịch thấp tải:

```sql
-- S1: non-destructive; minh họa, chạy trong pipeline có review
ALTER TABLE dbo.PartnerContact
ADD DisplayName nvarchar(200) NULL;
```

Chưa rename/drop `ContactName`. Trong giai đoạn này V1.0 vẫn đọc/ghi cột cũ, nên S1 tương thích với V1.0.

**Phase B — rollout V1.1 compatibility.** V1.1 vẫn **đọc `ContactName` authoritative** và ghi `ContactName` + `DisplayName` trong **cùng một UPDATE/transaction**, giữ `RowVersion` optimistic concurrency và `TenantId` filter. Ví dụ shape:

```sql
UPDATE dbo.PartnerContact
SET ContactName = @name,
    DisplayName = @name
WHERE Id = @id AND TenantId = @tenantId AND RowVersion = @expectedRowVersion;
-- 0 rows => 404/409 theo kiểm tra existence + tenant-safe policy.
```

Trong mixed V1.0/V1.1, V1.0 có thể cập nhật cột cũ đơn lẻ. Vì mọi reader vẫn dùng cột cũ, chưa có stale read do bản sao mới. Đo mismatch để quan sát nhưng **chưa chuyển read**.

**Phase C — drain V1.0 và backfill.** Gate: không còn V1.0 instances hoặc legacy writers khác; mọi writer active dual-write atomic. Backfill `DisplayName` theo batch nhỏ, idempotent, kiểm soát log growth và lock waits, có thể tạm dừng. SQL minh họa một batch, cần benchmark index và transaction scope trên dữ liệu thực:

```sql
;WITH batch AS (
  SELECT TOP (500) Id
  FROM dbo.PartnerContact WITH (UPDLOCK, READPAST, ROWLOCK)
  WHERE DisplayName IS NULL OR DisplayName <> ContactName
  ORDER BY Id
)
UPDATE c
SET DisplayName = c.ContactName
FROM dbo.PartnerContact AS c
JOIN batch AS b ON b.Id = c.Id;
```

`ROWLOCK` là hint, **không** bảo đảm SQL Server luôn dùng row locks; đánh giá blocking/deadlock. Cẩn thận collation và null semantics nếu có. Backfill và application UPDATE phải serialize đúng trên cùng row; giữ `RowVersion` conflict behavior và theo dõi side effects của backfill lên concurrency tokens. Nếu token bump làm tăng 409, throttle và điều chỉnh batch window.

**Phase D — V2 read cutover.** Chỉ khi zero mismatches có xác nhận sau backfill, tất cả writers active dual-write, contract tests pass, V2 mới đọc `DisplayName` (fallback `ContactName` trong migration window nếu cần), nhưng **tiếp tục dual-write**. Rollback V2 về **V1.1**, không về V1.0, vì V1.1 hiểu S1 và giữ dual-write. Giữ `ContactName` cho rollback. Với rollback target V1.1, reader trở lại cột cũ nhưng writes của V2 vẫn cập nhật cả hai nên không mất dữ liệu.

**Phase E — contract.** Sau 24h (đổi yêu cầu thành 72h) và khi không còn V1.1/V1.0 hay downstream dependency, release V2.x không đọc/ghi cột cũ, rồi drop `ContactName` ở một deployment tách biệt. Gate cần evidence từ query telemetry, application version inventory, consumers, rollback policy. Sau drop, không thể rollback về artifact cần cột cũ; xác lập rollback floor mới.

## 5. Why this works

Expand giữ contract cũ hoạt động. Chưa chuyển reader sang cột mới khi còn old-only writers. Sau khi mọi writer dual-write và backfill đạt parity, cả hai cột cùng đại diện cho một giá trị. Dual-write trong một DB transaction bảo vệ rollback. Contract chỉ xóa compatibility sau khi không còn dependency.

## 6. Verification plan

| Test | Setup | Expected |
|---|---|---|
| S1 + V1.0/V1.1 | Mixed read/write | V1.0 đọc được dữ liệu vừa cập nhật; không SQL 207 |
| Old writer after backfill | V1.0 update `ContactName` | Reader vẫn dùng authoritative old field; chưa cutover |
| V1.1 concurrent writers | Cùng `RowVersion` | Một update thành công, còn lại 409; hai cột bằng nhau |
| Backfill interrupted | Kill batch giữa các commits | Rerun hội tụ, không mất user writes |
| S1 + V1.1/V2 | Mixed writes | Parity duy trì; V2 read mới nhất |
| V2 → V1.1 rollback | Giữ S1 | API vẫn đúng; không restore DB |
| Schema deployment lag | V1.1 trước S1 | Pipeline phải block deployment, không phát traffic |
| Tenant boundary | Sai TenantId | Không đọc/ghi chéo tenant |
| Contract gate | Một consumer còn truy vấn old field | Không drop column |

Thực thi trên staging có traffic simulation trước production; verify row parity, `sql_exception_total` theo version, `backfill_remaining`, deadlocks, lock waits, p95, 409 rates, replication/backup impact. Tránh log tên liên hệ.

## 7. Alternative fixes

- **DB trigger/view compatibility:** Có thể bảo vệ old-only writers lâu hơn, nhưng tăng hidden write behavior, contention, test burden và rủi ro recursive/ordering; chỉ dùng khi app không thể upgrade đồng bộ và DBA đồng ý.
- **Versioned view/API mapping:** Hữu ích để giữ interface ổn định, nhưng vẫn phải giải quyết data synchronization và quyền deploy.
- **Brief maintenance window:** Có thể hợp lý nếu business chấp nhận downtime; không đáp ứng ràng buộc hiện tại.
- **Chỉ đổi tên ở DTO:** Nếu yêu cầu chỉ là tên hiển thị public, có thể không cần đổi physical DB column; ít rủi ro hơn. Hỏi lại product về business necessity của schema change.

## 8. Wrong / tempting fixes

- Rename `sp_rename` trước rồi rollback app khi lỗi: rollback app không phục hồi physical schema.
- Add new column + backfill một lần rồi V2 đọc new ngay: old-only writer vẫn có thể cập nhật cột cũ khiến dữ liệu stale.
- Dual-write ở hai request/transaction độc lập: partial failure có thể khiến hai cột lệch.
- Dùng database restore để rollback app: làm mất committed writes kể từ backup.
- Chỉ kiểm tra GET happy path trên một pod: bỏ sót writes, mixed versions và rollback.
- Drop legacy column ngay khi 100% V2 healthy: rollback window và consumers khác vẫn có thể phụ thuộc cột cũ.

## 9. Production implications & trade-offs

Chi phí của expand–contract là thời gian giữ hai cột, dual-write code, migration monitoring, batch tuning và nhiều release gates. Đổi lại là giảm blast radius của schema migration, rollback ứng dụng khả thi và không cần downtime. DDL additive vẫn có schema modification locks; cần đo thời gian và timeout. Backfill có thể tăng log, lock contention và `RowVersion` churn. Giữ compatibility lâu hơn tốn công vận hành nhưng cần thiết cho 72h rollback. Không thể cam kết zero downtime nếu chưa kiểm chứng các dependency và lock behavior trên staging giống production.

## 10. What a Senior engineer should notice

Schema là **shared runtime contract** giữa các version đang sống, không phải artifact riêng của một release. Thứ tự release quan trọng bằng code correctness. Senior engineer cần xác định authority, rollback floor, migration ownership, no-go gates và failure recovery trước khi chạy DDL; tránh biến backfill thành race hoặc lấy green deployment làm bằng chứng data correctness.

## 11. Requirement change: 72h rollback

Kéo dài thời gian giữ `ContactName`, V1.1 rollback artifact, dual-write và telemetry gate tối thiểu 72 giờ kể từ cutover (hoặc theo chính sách release cụ thể). Hoãn phase E tương ứng, kiểm tra storage/log overhead và không cho deploy cleanup phá rollback floor. Nếu compliance đòi rollback dữ liệu, đó là một yêu cầu khác cần thiết kế audit/compensating operations, không thể suy ra từ app rollback.
