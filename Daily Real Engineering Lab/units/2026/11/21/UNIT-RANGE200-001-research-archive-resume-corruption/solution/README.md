# Reference Solution — spoiler

## Symptoms
Resume thành công ở transport nhưng bản kết quả có prefix lặp.

## Evidence
Status 200 trả toàn bộ representation; offset cũ vẫn được ghép vào body mới.

## Root cause
Origin/proxy được phép bỏ qua Range. HTTP 200 không phải suffix; 206 mới biểu thị partial content.

## Why fix works
Bản tham chiếu `Program.cs` thay partial bằng full body khi 200; với 206 kiểm tra offset và tổng kích thước trước khi nối; từ chối status khác.

## Verification
Chỉnh `starter/Program.cs` rồi chạy `./verify.ps1`. Thử nhánh `ignoreRange:false` để kiểm tra 206, sau đó trả về true.

## Alternatives
Restart download hoàn toàn; dùng If-Range với strong ETag và checksum toàn file; temp file và atomic rename.

## Wrong fixes
Bỏ checksum chỉ che giấu corruption. Cắt body dựa trên offset mà không xác minh ETag có thể trộn hai phiên bản. Nối mọi response 2xx là sai contract.

## Production implications
Log status, Content-Range, ETag, kích thước và checksum (không log payload nhạy cảm). Xử lý 416 riêng.

## Trade-offs
Restart đơn giản nhưng tốn bandwidth; resume yêu cầu validator, chống thay đổi representation và bảo vệ file chưa hoàn tất.

## Senior insight
Kiểm tra identity của representation, atomic publication, retry giới hạn và checksum end-to-end. Lab dùng fake origin in-memory để cô lập semantics.
