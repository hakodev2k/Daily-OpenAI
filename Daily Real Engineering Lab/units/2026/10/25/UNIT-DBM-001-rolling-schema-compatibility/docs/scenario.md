# Scenario & constraints — Partner Directory

## Hệ thống

- ASP.NET Core 8 API, 4 pods, rolling deployment từng pod (tối đa 2 phiên bản cùng lúc), SQL Server 2022.
- Bảng `PartnerContact(Id bigint PK, TenantId uniqueidentifier, ContactName nvarchar(200) NOT NULL, RowVersion rowversion)`; 2 triệu rows.
- V1.0 đọc/ghi `ContactName`; một dashboard legacy cũng đọc trường này qua API V1.
- V2 dự kiến chuyển nội bộ sang `DisplayName nvarchar(200)`, giữ JSON API response `name` ổn định; không yêu cầu đổi public API.
- Traffic 40 writes/s, 500 reads/s giờ cao điểm; p95 API < 200 ms. Không được ngừng ghi để backfill.
- Mỗi record update là replace một tên, với optimistic concurrency bằng `RowVersion`; conflict hợp lệ trả 409 và client retry có kiểm soát.
- Triển khai app có thể rollback trong 24 giờ; DB backup/restore không được dùng như cơ chế rollback release vì sẽ làm mất writes hợp lệ.
- DDL cần được triển khai qua pipeline riêng; team có 2 backend engineers + 1 DBA, không có distributed transaction coordinator.
- Mọi phương án phải giữ tenant isolation và không log PII từ tên liên hệ.

## Business acceptance

1. Trong suốt rolling deployment, mọi version còn được phép phục vụ traffic phải đọc đúng giá trị cập nhật mới nhất theo `RowVersion`.
2. Không có request 5xx do mismatch schema ở mọi trạng thái đã phê duyệt.
3. Rollback ứng dụng trong 24 giờ không đòi restore DB và không làm mất các cập nhật đã commit.
4. Backfill chạy nhiều batch, có thể dừng/tiếp tục; đo lock wait, deadlock, log growth và số rows chưa đồng bộ.
5. Có release gate rõ ràng để loại bỏ cột legacy sau khi rollback window đóng.

## Nhiệm vụ

A. Vẽ ma trận V1.0, V1.1 compatibility, V2 × schema S0, S1 (cả hai cột), S2 (chỉ cột mới). Chỉ đánh dấu tương thích nếu **read và write** đều đúng dưới concurrent traffic.

B. Chọn thứ tự DDL/app deploy, nguồn dữ liệu authoritative tại từng phase, chiến lược backfill, và điều kiện cutover. Xem xét một writer V1.0 cập nhật sau khi row đã backfill.

C. Thiết kế rollback target thực tế (V1.0 hay V1.1?) và giải thích tác động khi một pod còn chạy V2.

D. Đề xuất ít nhất 6 kiểm thử, bao gồm concurrent writers, schema lag, partial rollout, rollback, backfill interruption, tenant isolation.

E. So sánh ít nhất 2 lựa chọn: rename/drop ngay, expand–migrate–contract (có thể cân nhắc trigger, view hoặc dual-write) và chi phí vận hành.

## Thay đổi yêu cầu

Sau khi bạn đã viết kế hoạch: security yêu cầu giữ khả năng rollback trong **72 giờ** thay vì 24 giờ. Cập nhật gate contract và chính sách retention tương ứng; không thay đổi dữ liệu bằng backup restore.
