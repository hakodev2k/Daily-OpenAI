# Reference Solution — chỉ xem sau khi tự thử

## Symptoms & evidence
Các chunk nhỏ làm ký tự nhiều byte thành U+FFFD. Byte payload ban đầu không đổi; kích thước chunk quyết định tỷ lệ lỗi. Đây không phải lỗi font hay proxy tự thay byte.

## Root cause
`Encoding.UTF8.GetString(bytes, offset, count)` được gọi độc lập cho từng chunk. Khi một code point UTF-8 bị chia đôi, mỗi lần decode coi chunk như một văn bản hoàn chỉnh và phát sinh replacement character. Ranh giới network read không phải ranh giới ký tự.

## Why the fix works
Dùng một `Decoder` xuyên suốt các chunk, gọi `GetChars` với `flush: false` cho các chunk trung gian và `flush: true` ở chunk cuối. Decoder giữ các byte chưa đủ để tạo ký tự. Xem `Program.cs` ở thư mục này và áp dụng ý tưởng vào `starter/Program.cs`.

## Verify
Chạy `./reproduce.ps1` trước sửa, ghi evidence; sau sửa chạy `./verify.ps1`. Harness thử 7 kích thước chunk và input chứa byte UTF-8 không hợp lệ. Nếu input rỗng cần đảm bảo semantics decode rỗng đúng; production streaming có thể cần flush cuối riêng khi stream kết thúc.

## Alternative fixes
Buffer toàn bộ bytes rồi decode một lần phù hợp payload nhỏ, có giới hạn rõ ràng. Với stream lớn hoặc liên tục, decoder stateful tránh giữ toàn bộ payload trong RAM. `StreamReader` có thể phù hợp khi ownership/encoding và buffering được quản lý đúng.

## Wrong / tempting fixes
Tăng chunk size chỉ làm giảm xác suất, không bảo đảm ký tự không bị cắt. Thay font không phục hồi byte đã decode sai. Loại bỏ U+FFFD che giấu mất dữ liệu.

## Production implications & trade-offs
Phân biệt message framing (độ dài/điểm kết thúc bản tin) và character decoding; đừng dùng một lần `ReadAsync` như một bản tin hoàn chỉnh. Với dữ liệu cần tính toàn vẹn tuyệt đối, cân nhắc `new UTF8Encoding(false, true)` để reject malformed input thay vì replacement, kèm dead-letter/telemetry. Tách log dữ liệu nhạy cảm khỏi byte dumps.

## Senior engineer should notice
Test theo fragmentation adversarial, đặc biệt split trong multibyte code point; đo số byte, số ký tự thay thế, checksum và phân loại corruption upstream/downstream. Kiểm tra cancellation, buffer ownership và giới hạn message size trước khi đưa vào production.
