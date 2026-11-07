> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## Symptoms
Input có dữ liệu và inspection thành công, nhưng storage nhận 0 byte.

## Evidence
`InputBytes` lớn hơn 0, checksum tồn tại, còn `UploadedBytes=0`. Không có exception ở storage adapter.

## Root cause
Bước inspection đọc hết seekable stream. Cùng stream được chuyển tiếp sang upload mà position vẫn ở cuối, nên `CopyToAsync` bắt đầu từ EOF.

## Why the fix works
Mỗi stage phải có contract rõ về stream position. Với pipeline hiện tại, sau inspection có thể đưa seekable stream về vị trí bắt đầu trước khi stage upload đọc nó.

## How to verify
Trong `starter/Program.cs`, sau inspection và trước upload, bảo đảm stream ở đúng vị trí mà upload contract yêu cầu. Chạy `./verify.ps1`; script kiểm tra byte count, payload equality và việc inspection vẫn tồn tại.

## Alternative fixes
- Tạo stream độc lập cho từng consumer nếu payload nhỏ và ownership rõ.
- Thiết kế API nhận immutable bytes/file abstraction thay vì chia sẻ mutable stream state.
- Với stream không seek được, buffer có giới hạn hoặc pipeline dữ liệu theo một pass tùy kích thước và yêu cầu.

## Wrong or misleading fixes
- Sửa storage adapter để tự động seek về 0 có thể che contract sai và phá trường hợp caller cố ý upload từ current position.
- Bỏ inspection làm mất requirement nghiệp vụ.
- Catch exception không giúp vì flow hiện tại không nhất thiết phát sinh exception.

## Production implications
File lớn cần tránh copy toàn bộ vào memory. Cần định nghĩa ownership, seekability, position và disposal ở boundary. Integration test nên kiểm tra payload thực, không chỉ HTTP/status thành công.

## Trade-offs
Reset position đơn giản khi stream seekable và contract cho phép. Buffering hỗ trợ nhiều consumer nhưng tăng memory/I/O. Streaming một pass tiết kiệm tài nguyên nhưng cần thiết kế các stage phối hợp.

## What a Senior engineer should notice
`Stream` là mutable stateful resource. API nhận `Stream` cần contract về ownership, lifetime, seekability và expected position; nếu contract này ngầm định, bug thường xuất hiện khi thêm middleware/inspection stage.