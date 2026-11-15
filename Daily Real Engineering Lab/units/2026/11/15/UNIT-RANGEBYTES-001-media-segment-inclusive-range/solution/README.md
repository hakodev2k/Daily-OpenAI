# Reference Solution — chỉ xem sau khi tự reproduce và thử fix

## Symptoms
Ba đoạn có metadata hợp lệ về hình thức nhưng payload ghép lại ngắn hơn 101 byte; SHA256 khác.

## Evidence
Các đoạn `0-31`, `32-63`, `64-100` lần lượt phải dài 32, 32, 37 byte. Starter tạo 31, 31, 36 byte; mỗi đoạn thiếu byte cuối.

## Root cause
HTTP byte range là inclusive ở cả hai đầu, trong khi `AsSpan(start, length)` nhận độ dài, không nhận end index. Starter truyền `end-start`, bỏ mất byte tại `end`.

## Reference patch
Trong `starter/Program.cs`, thay:
```csharp
var length = end - start;
```
bằng:
```csharp
var length = checked(end - start + 1);
```
Giữ nguyên range validation và các assertions. Đoạn `0-0` phải trả một byte, `0-100` trả toàn bộ file.

## Why the fix works
Invariant: `length == end-start+1` với `0 <= start <= end < data.Length`. `AsSpan` nhận đúng số byte. `checked` là biện pháp phòng vệ nếu sau này dùng index kiểu lớn hơn hoặc thay đổi validation.

## How to verify
Chạy `./scripts/verify.ps1`. Kiểm tra SHA256 và boundary cases, rồi chạy thêm tải một đoạn duy nhất. Khi tích hợp HTTP thật, kiểm tra 206 Partial Content, `Content-Range`, `Content-Length`, 416 cho range không hợp lệ, và `If-Range` với ETag.

## Alternative fixes
Có thể dùng `data[start..(end+1)]` sau validation, nhưng cần xem kỹ overflow và allocation; hoặc thiết kế API nội bộ dùng `(offset,count)` rồi chuyển đổi tại boundary HTTP.

## Wrong fixes
Bỏ byte cuối khỏi checksum hoặc nối thêm byte 0 chỉ che triệu chứng. Thêm retry không giải quyết sai contract. Thay đổi client để dùng range chồng lấp làm hệ thống phụ thuộc workaround không chuẩn.

## Production implications
Giữ một source of truth cho range parsing, giới hạn số range/request và kích thước response, xác thực quyền tải, không log nội dung nhạy cảm. Với file có thể thay đổi, sử dụng ETag/If-Range để tránh ghép các phiên bản khác nhau.

## Trade-offs
Copy từng segment đơn giản và an toàn cho bài lab nhưng tốn allocation; streaming với bounded buffers phù hợp file lớn hơn, cần quản lý lifetime và backpressure. Không tối ưu trước khi có số đo.

## Senior engineer should notice
Checksum kiểm tra tính toàn vẹn toàn file nhưng chưa đủ định vị lỗi; assertions tại boundary và invariant của giao thức giúp khoanh vùng nhanh hơn. Phải kiểm thử metadata cùng bytes chứ không chỉ status 206.
