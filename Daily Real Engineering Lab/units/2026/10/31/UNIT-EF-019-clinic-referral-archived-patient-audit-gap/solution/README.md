# Reference Solution — spoiler

## Root cause
`ReferralAuditExporter` tải `ReferralEvent` bằng `Include(x => x.Patient)` trong khi navigation `Patient` được khai báo required và `Patient` có global query filter loại các hồ sơ archived. EF Core có thể sinh `INNER JOIN` tới tập principal đã lọc; dependent `EVT-102` còn trong bảng nhưng bị loại khỏi kết quả export.

## Fix tối thiểu
Thay toàn bộ class `ReferralAuditExporter` trong `starter/Program.cs` bằng class trong [ReferenceQuery.cs](ReferenceQuery.cs) (không giữ hai class trùng tên). Query chỉ project các trường audit cần thiết, lọc tenant trên bảng audit, không `Include` principal, và dùng `AsNoTracking()`.

## Verify
```powershell
./verify.ps1
```
Mong đợi: hai ID của tenant, không có ID tenant khác, không có `Patient` tracked. Đây là kiểm chứng trên starter vừa chỉnh, không phải chạy reference độc lập.

## Wrong fixes
- `IgnoreQueryFilters()` trên toàn query: có thể bỏ qua các policy bảo mật/tenant khác, không phù hợp với mục tiêu audit tối thiểu.
- Đổi quan hệ thành optional chỉ để ép `LEFT JOIN`: thay đổi mô hình dữ liệu, nullability và hành vi của các luồng khác.
- Bỏ `EVT-102` khỏi expected count: làm xanh test bằng cách thay hợp đồng nghiệp vụ.
- Xóa `IsArchived` khỏi hồ sơ: làm lộ hồ sơ đã lưu trữ và thay đổi chính sách truy cập.
- Dùng `AsNoTracking()` nhưng vẫn `Include`: thay đổi tracking, không giải quyết cardinality của JOIN.

## Trade-offs
Projection DTO độc lập với vòng đời `Patient`, giảm cột đọc và tránh tải PII. Đổi lại, nếu báo cáo tương lai cần tên bệnh nhân tại thời điểm phát sinh sự kiện, cần thiết kế snapshot có kiểm soát hoặc nguồn dữ liệu lịch sử được phân quyền riêng, không nên mở toàn bộ filter.

## Verification boundary
Không khẳng định đã chạy build/runtime; `validationMode=static-review`. Người học phải chạy scripts trong môi trường .NET 8.
