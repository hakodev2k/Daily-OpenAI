# Reference Solution — chỉ xem sau khi tự reproduce và thử fix

## Symptoms
Export trả về byte[] không rỗng, không có exception phía producer, nhưng `ZipArchiveMode.Read` không mở được cả gói thông thường lẫn gói rỗng.

## Evidence
Bộ đọc báo `InvalidDataException` ngay lúc mở container. Kiểm tra nội dung từng entry chưa bắt đầu; dữ liệu rỗng cũng lỗi. Đây là dấu hiệu cấu trúc ZIP chưa hoàn chỉnh, không phải lỗi encoding hoặc thiếu tài liệu.

## Root cause
`CreatePackage` dùng `using var archive` rồi `return buffer.ToArray()` trong cùng scope. Biểu thức return được **đánh giá trước** khi `using var` được dispose. `ZipArchive` ở chế độ Create chỉ ghi cấu trúc kết thúc/central directory khi archive được dispose. Vì vậy byte[] được chụp quá sớm, dù archive sau đó vẫn được dispose khi rời method.

## Why the fix works
Đặt `ZipArchive` trong một block `using (...)` kết thúc **trước** `buffer.ToArray()`. `leaveOpen: true` giữ `MemoryStream` mở để lấy bytes sau khi hoàn tất ZIP. Không cần đổi encoding, compression level hay format tài liệu.

## How to verify
Chạy `./scripts/verify.ps1` trên `starter/` đã sửa. Để kiểm tra reference độc lập: `./scripts/check-reference.ps1`. Cả hai đường chạy dùng cùng contract: ZIP hợp lệ, đúng entry, đúng nội dung và ZIP rỗng hợp lệ. Reference không thay thế việc verify bài làm của bạn.

## Alternative fixes
- Tách logic ghi archive thành helper hoàn tất scope trước khi trả bytes.
- Với file lớn, ghi ra temporary file/stream và hoàn tất ZIP trước khi publish file atomically; tránh `MemoryStream` cho payload rất lớn.
- Nếu streaming response trực tiếp, phải bảo đảm archive writer kết thúc đúng cách **trước** khi coi response là hoàn tất; cần xử lý client disconnect và partial download.

## Wrong / tempting fixes
- `Flush()` trên entry/stream: không phải bằng chứng central directory đã được ghi.
- Tăng `MemoryStream.Capacity`: chỉ thay dung lượng bộ nhớ, không hoàn tất cấu trúc ZIP.
- Đổi `CompressionLevel` hoặc UTF-8: không xử lý thời điểm finalize container.
- Catch `InvalidDataException` phía consumer: che giấu file hỏng, không sửa export.

## Production implications
Đừng báo “export complete” trước khi artifact hoàn chỉnh; với object storage, nên upload từ file/stream đã finalized, kiểm tra checksum và chỉ publish download link sau khi xác nhận thành công. Giới hạn kích thước, số entry và tên entry theo input trust boundary; tránh zip-slip khi consumer extract. Đây không phải lab về bypass validation hoặc bảo mật archive.

## Trade-offs
`MemoryStream` đơn giản và local-first, nhưng tăng memory theo kích thước payload; file tạm hoặc streaming thích hợp hơn với archive lớn nhưng đòi hỏi cleanup, retry và failure semantics rõ ràng.

## Senior engineer should notice
Resource lifetime là một phần của data format contract. Kiểm thử cần có **consumer độc lập** mở artifact, bao gồm empty case; không chỉ kiểm tra “không throw” ở producer. Xác định chính xác thời điểm artifact trở nên publishable.

## Authoritative references
- .NET ZipArchive: https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive
- ZipArchive.Dispose: https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive.dispose
- C# using statement: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/using
