# Reference Solution — chỉ xem sau khi tự sửa

## Symptoms & Evidence
Báo cáo có 8 byte nội dung nhưng code gửi 128 byte; checksum của response không khớp với nội dung logic. `--check` thất bại có thể tái hiện độc lập.

## Root Cause
`MemoryStream.GetBuffer()` cung cấp toàn bộ backing array, có kích thước theo `Capacity`, không nhất thiết bằng `Length`. Buffer dư không phải là dữ liệu hợp lệ. Trong các luồng dùng lại buffer, vùng dư còn có thể chứa dữ liệu cũ.

## Why Fix Works
`ToArray()` sao chép đúng các byte hợp lệ từ stream thành array riêng. Khi stream còn sống và muốn giảm allocation, có thể dùng `TryGetBuffer(out ArraySegment<byte>)` nhưng phải tôn trọng `Offset` và `Count`, không bao giờ gửi cả `Array`.

## Verification
Chạy `./verify.ps1` sau khi tự sửa `starter`; chạy `dotnet run --project solution/ExportProbe.csproj -- --check` để kiểm tra reference độc lập. Kỳ vọng cả hai trả exit code 0 và exported length = 8.

## Alternative Fixes & Trade-offs
- `ToArray()`: đơn giản, sở hữu dữ liệu độc lập, thêm allocation/copy.
- `TryGetBuffer` + slice: tránh copy nhưng cần kiểm soát lifetime, offset, count và khả năng expose dữ liệu nhạy cảm.
- Stream trực tiếp sang HTTP response: có thể giảm allocation, nhưng cần reset `Position` và xác định đúng `Content-Length` nếu giao thức yêu cầu.

## Wrong / Tempting Fixes
- Cắt chuỗi sau khi UTF-8 decode: làm hỏng binary và không sửa hợp đồng byte.
- Hash toàn bộ backing array để “đồng bộ checksum”: hợp thức hóa response sai.
- Giảm capacity bằng đúng payload size: tình cờ qua test nhưng không đảm bảo đúng khi kích thước thay đổi.

## Production Implications
Giám sát response length và content hash trên boundary, tránh log raw payload nhạy cảm, kiểm thử với capacity lớn hơn length, empty payload và dữ liệu binary chứa NUL.

## Senior Engineer Should Notice
Một API trả `byte[]` không bảo đảm toàn bộ mảng là nội dung hợp lệ. Cần phân biệt logical length, buffer ownership và dữ liệu được phép rời khỏi trust boundary.
