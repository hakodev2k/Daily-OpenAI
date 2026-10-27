# Reference Solution — CHỈ ĐỌC SAU KHI TỰ LÀM

## Symptoms
Partner gửi hai event ID liền nhau và một retry đúng nghĩa; hệ thống ghi nhận thiếu event, thiếu usage units nhưng không báo JSON lỗi. Fixture ID nhỏ không phát hiện được.

## Evidence
Fixture chính có `9007199254740992` và `9007199254740993`, đều nằm trong miền `Int64`. Starter trả về một ID sau import. Fixture ID lẻ lớn cũng bị thay đổi. `HashSet<long>` chỉ là nơi triệu chứng xuất hiện.

## Root cause
`JsonElement.GetDouble()` chuyển JSON integer sang IEEE-754 binary64; các integer trên `2^53` không còn được biểu diễn chính xác liên tiếp. Ép `double` trở lại `long` không phục hồi bit đã mất. Hai ID khác nhau có thể cùng một key dedup, dẫn đến underbilling.

## Reference implementation
Xem [UsageImporter.cs](UsageImporter.cs). Thay đọc ID qua `double` bằng `GetInt64()`, giữ `long` xuyên suốt đường import/dedup. Không đổi logic `HashSet` vốn xử lý retry đúng.

## Why the fix works
`GetInt64()` đọc integer token trực tiếp theo miền `Int64` và không đi qua biểu diễn dấu phẩy động. Giá trị vượt miền hoặc token không hợp lệ phải được xử lý như lỗi contract thay vì âm thầm làm tròn.

## How to verify
1. Sửa `starter/UsageImporter.cs` theo hướng của bạn, chạy `./verify.ps1` (bắt buộc).
2. Để kiểm tra riêng reference: `dotnet run --project solution/RoamingUsage.Reference.csproj -- --check`. Reference dùng cùng fixture từ starter Program, nhưng **không** thay thế verification của learner.

## Alternative fixes
- Deserialization trực tiếp sang DTO có `long EventId` và strict contract validation: hợp lý nếu toàn bộ payload có schema ổn định.
- Đọc ID dưới dạng string nếu upstream thực sự dùng opaque identifiers; khi đó hợp đồng wire format phải quy định rõ JSON string và canonicalization.
- `decimal` có thể lưu integer ở dải này, nhưng không cần thiết và làm contract identity kém rõ hơn `long`.

## Wrong / tempting fixes
- Thêm `DistinctBy` hoặc đổi `HashSet` sang `Dictionary`: không cứu được ID đã mất precision trước dedup.
- Làm tròn theo cách khác hoặc cast lại `long`: không phục hồi giá trị ban đầu.
- Dùng tolerance so sánh số thực: không phù hợp với định danh exact-match.
- Chỉ tăng log level: giúp phát hiện nhưng không sửa corruption.

## Production implications and trade-offs
- Audit mọi bước ETL, Kafka/JSON gateways và các JavaScript consumers: JSON number được JS biểu diễn bằng `Number` có cùng giới hạn safe integer; có thể cần chuyển ID thành string **theo contract được version hóa**.
- Tạo metric mismatch giữa số event nhận, số event unique và tổng units; lưu raw payload trong vùng kiểm soát truy cập để forensic, tránh log PII/secret.
- Với payload cũ đã xử lý sai, cần chiến lược replay từ raw immutable events và đối soát theo business rules, không tự động ghi đè hóa đơn.
- Parse strict có thể từ chối payload trước đây được chấp nhận; rollout cần đo tỷ lệ invalid và phối hợp với đối tác.

## Senior engineer should notice
Bug không nằm ở dedup algorithm mà ở boundary type conversion. Cần định nghĩa rõ `eventId` là **identity** (exact) thay vì measurement (approximate), rồi bảo toàn invariant qua mọi serialization boundary. Regression phải chứa ID ngay trên `2^53`, duplicate thật và ID nhỏ.

## Official references
- [JsonElement.GetInt64](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.getint64)
- [JsonElement.GetDouble](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.getdouble)
- [IEEE-754 binary floating-point types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types)
