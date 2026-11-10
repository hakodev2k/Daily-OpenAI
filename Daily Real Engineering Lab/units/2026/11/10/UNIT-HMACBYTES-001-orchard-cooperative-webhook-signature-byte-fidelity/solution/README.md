# Reference Solution — spoiler

## Symptoms & evidence
Hai payload có cùng dữ liệu JSON nhưng khác byte-level representation. Sender ký chính xác byte payload đã gửi. Bản starter chấp nhận compact nhưng từ chối pretty.

## Root cause
Receiver parse JSON rồi serialize lại trước khi HMAC; bước này có thể thay đổi whitespace và thứ tự property, nên dữ liệu được ký không còn là dữ liệu trên wire.

## Why the fix works
HMAC-SHA256 được tính trực tiếp từ raw request bytes trước khi deserialize. So sánh chữ ký dùng `CryptographicOperations.FixedTimeEquals` và kiểm tra đúng độ dài. Tham khảo `solution/Program.cs`.

## Verify
Chép phần chỉnh sửa sang `starter/Program.cs` (không thay toàn bộ test harness), rồi chạy `./verify.ps1`. Cần pass cả hai positive và ba negative cases. Có thể dùng `dotnet run --project ./starter/WebhookLab.csproj -- --mode verify`.

## Alternative fixes
Nếu đối tác **quy định rõ** một JSON canonicalization scheme thì cả hai phía có thể ký canonical form; không tự đặt canonicalization riêng. Một số giao thức ký cả timestamp, request path và body; phải tuân thủ chính xác contract của đối tác.

## Wrong / tempting fixes
- Bỏ qua xác thực với pretty JSON: mở cửa cho giả mạo.
- Sort property rồi ký: không sửa được sai lệch nếu sender ký raw bytes.
- Dùng so sánh chuỗi thông thường: bỏ mất hardening timing-side-channel.
- Chỉ tăng retry: cùng payload vẫn sai.

## Production implications & trade-offs
Giới hạn kích thước body; xác thực trước xử lý nghiệp vụ; giữ raw bytes an toàn qua middleware; tránh log body chứa PII; dùng secret store và rotation có kiểm soát; bổ sung timestamp/replay protection theo giao thức. Raw-byte signature nhạy với mọi proxy biến đổi body, cần contract rõ về content encoding. Senior engineer phải phân biệt integrity, authenticity và replay prevention.
