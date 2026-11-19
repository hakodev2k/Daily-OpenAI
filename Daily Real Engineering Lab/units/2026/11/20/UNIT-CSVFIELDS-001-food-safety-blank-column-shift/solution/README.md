# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Bản ghi có kết quả đo trống bị coi là thiếu trường, dù cấu trúc có đủ bốn vị trí.

## Evidence
Input `S-102,South,,PENDING` có bốn cột; starter trả `FormatException` vì mảng sau split còn ba phần tử. File `solution/Program.cs` là mã tham chiếu.

## Root cause
`StringSplitOptions.RemoveEmptyEntries` xóa trường rỗng có ý nghĩa vị trí. Điều kiện `parts.Length < 4` cũng cho phép trường thừa đi qua mà không được cảnh báo.

## Why the fix works
Dùng `StringSplitOptions.TrimEntries` nhưng không xóa empty entries; yêu cầu chính xác bốn trường. Contract hiện tại chỉ cho phép CSV đơn giản, không có dấu phẩy trong trường quoted.

## How to verify
Áp dụng thay đổi vào `starter/Program.cs`, chạy `./verify.ps1`; kiểm tra `S-102` và `malformedRejected=True`. Không dùng solution thay cho code người học.

## Alternative fixes
Dùng parser CSV chuẩn nếu đối tác bổ sung quoted fields, escaping hoặc multiline. Định nghĩa schema typed, validation và reject reason có cấu trúc cho pipeline production.

## Wrong / tempting fixes
Gán mặc định cột thiếu sau khi split không khôi phục được vị trí đã mất. Bỏ qua bản ghi trống khiến mất dữ liệu hợp lệ. Chỉ sửa `< 4` thành `!= 4` vẫn từ chối trường rỗng.

## Production implications
Quarantine dòng không đúng contract, thống kê reject rate theo đối tác, lưu raw payload an toàn và kiểm tra idempotent import. Không ghi PII trong log.

## Trade-offs
Parser đơn giản ít phụ thuộc nhưng chỉ đúng với contract giới hạn; parser chuẩn tốn thêm dependency và cần quy định rõ null, empty và whitespace.

## What a Senior engineer should notice
Dữ liệu hợp lệ có thể chứa giá trị thiếu; tính toàn vẹn của vị trí cột là invariant. Test phải bao phủ empty ở đầu, giữa, cuối, extra field, quoted commas và thay đổi contract. Khi mở rộng CSV, thay parser thay vì tự chế quy tắc escaping.
