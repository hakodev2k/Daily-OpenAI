# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Một giờ hẹn hợp lệ theo mặt đồng hồ được xác nhận mà không xác định duy nhất instant UTC.

## Evidence
Bộ replay cho thấy `fall-fold` ACCEPT, trong khi hợp đồng đòi AMBIGUOUS. Giờ `01:30` ngày 01/11/2026 ở New York xuất hiện hai lần khi đồng hồ quay lùi.

## Root cause
`ConvertTimeToUtc` mặc định chọn một offset khi giờ local ambiguous; không thể suy ra ý định người đặt lịch từ wall-clock time đơn lẻ.

## Why fix works
Kiểm tra `IsInvalidTime` và `IsAmbiguousTime` trước khi convert; từ chối input không ánh xạ một-một. Chỉ tạo instant UTC cho thời điểm xác định duy nhất.

## Verify
`pwsh -File ./verify.ps1` kiểm tra **starter learner đã sửa**, không phải solution. Có thể tự chạy `dotnet run --project solution/ClinicBooking.csproj` để đối chiếu.

## Alternatives
Cho client gửi `DateTimeOffset` cùng timezone ID và kiểm tra offset thuộc các offset hợp lệ; hoặc yêu cầu chọn occurrence thứ nhất/thứ hai trong fold. Quyết định phải thể hiện rõ trong API contract.

## Wrong / tempting fixes
Cộng cứng một giờ làm thay đổi lịch ngoài DST và không giải quyết ambiguity; chuyển tất cả input thành `DateTimeKind.Utc` chỉ thay nhãn, không giải quyết ý định. Luôn chọn standard offset có thể là chính sách hợp lệ nếu được nghiệp vụ chấp thuận rõ ràng, nhưng không phù hợp hợp đồng lab.

## Production implications
Persist UTC instant, timezone ID và local display time; kiểm tra timezone rules khi đặt lịch; audit chính sách DST và thông báo người dùng khi có ambiguity.

## Trade-offs
Reject đơn giản, an toàn nhưng tăng thao tác người dùng. Explicit offset cải thiện UX nhưng đòi validation chặt và client phức tạp hơn.

## Senior insight
Không đồng nhất wall-clock time, timezone và instant; timezone rules có thể thay đổi theo luật, nên cần kiểm thử biên DST và hợp đồng API theo vùng.

## References
Microsoft Learn: https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo.isambiguoustime ; https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo.isinvalidtime
