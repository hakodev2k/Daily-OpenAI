# Reference Solution — chỉ đọc sau khi tự thử

## Symptoms & evidence
Ngày 2026-03-08 02:30 tại New York không tồn tại do đồng hồ nhảy 02:00→03:00; ngày 2026-11-01 01:30 xuất hiện hai lần. Baseline 2026-11-02 10:00 ứng với 15:00Z.

## Root cause
Starter chuyển local `DateTime` trực tiếp sang UTC mà không định nghĩa hợp đồng cho invalid/ambiguous wall-clock time. Trường `Offset` trong request bị bỏ qua; conversion có thể throw với invalid time và chọn offset mặc định cho ambiguous time.

## Why fix works
`solution/Program.cs` từ chối invalid time, yêu cầu offset cho ambiguous time, kiểm tra offset thuộc tập offset thực tế của zone rồi tạo `DateTimeOffset` trước khi chuyển UTC. Ngày thường vẫn được suy ra offset duy nhất; offset sai bị từ chối.

## Verify
Chỉnh `starter/Program.cs` của bạn; chạy `pwsh ./scripts/verify.ps1`. Có thể so sánh với `solution/Program.cs` sau đó. Không sửa script verify để làm test xanh.

## Alternatives
- API chỉ nhận UTC instant: đơn giản cho backend nhưng UI phải chuyển đổi và xác nhận timezone chính xác.
- API nhận local + zone + explicit offset: thể hiện lựa chọn người dùng tại giờ trùng.
- Policy tự dời giờ không tồn tại: chỉ hợp lệ khi business cho phép và phải công khai, audit.

## Wrong fixes
- Cộng/trừ cố định 5 giờ: sai nửa năm.
- Lưu `DateTimeKind.Local` theo timezone máy chủ: phụ thuộc deployment.
- Tự động chọn standard offset trong overlap: mất ý định người đặt.
- Bắt mọi exception rồi trả HTTP 500: che mất validation contract.

## Production implications
Lưu UTC instant, IANA zone, original local input và selected offset để audit; hiển thị timezone abbreviation/offset; version hóa timezone rules khi cần replay; kiểm tra dữ liệu lịch cũ trước migration.

## Trade-offs / Senior judgment
Quy định rõ invalid/ambiguous policy ở API contract, không mặc định chọn một instant khi business cần phân biệt. Thử cả hai mùa, nhiều zone và lịch cập nhật timezone database. `static-review` nghĩa là chưa chạy .NET/PowerShell trong pipeline tạo lab.

## References
- https://learn.microsoft.com/dotnet/api/system.timezoneinfo.isinvalidtime
- https://learn.microsoft.com/dotnet/api/system.timezoneinfo.isambiguoustime
- https://learn.microsoft.com/dotnet/api/system.datetimeoffset
