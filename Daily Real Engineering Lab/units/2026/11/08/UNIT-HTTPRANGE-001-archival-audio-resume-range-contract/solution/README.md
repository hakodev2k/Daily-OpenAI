# Reference Solution — chỉ đọc sau khi điều tra

## Symptoms / Evidence
Server tôn trọng Range trả 206 với Content-Range bắt đầu ở byte 32, kết quả 128 byte hợp lệ. Server bỏ qua Range trả 200 toàn bộ 128 byte; starter append vào 32 byte cũ thành 160 byte, SHA-256 không khớp.

## Root cause
Client coi mọi HTTP 2xx là phần dữ liệu tiếp theo. HTTP 200 cho request Range không chứng minh server đã trả phần còn thiếu.

## Fix và tại sao hoạt động
Kiểm tra status trước khi ghi: với 206, yêu cầu Content-Range có From đúng offset hiện có, To phù hợp và Length khớp tài nguyên đã biết; chỉ khi đó append. Với 200, xóa/truncate nội dung cũ rồi ghi response từ đầu. Với status không mong đợi, từ chối. Tránh ghi file thật trước khi validation hoàn tất.

Trong `starter/Program.cs`, thay phần bắt đầu từ `using var output` đến trước `Console.WriteLine` bằng:

```csharp
using var output = new MemoryStream();
if (response.StatusCode == HttpStatusCode.PartialContent)
{
    var range = response.Content.Headers.ContentRange;
    if (range?.From != existing.Length || range.To != original.Length - 1 || range.Length != original.Length)
        throw new InvalidDataException("Unexpected Content-Range");
    await output.WriteAsync(existing);
}
else if (response.StatusCode != HttpStatusCode.OK)
{
    throw new InvalidDataException("Unexpected status");
}
await response.Content.CopyToAsync(output);
var actual = output.ToArray();
```

## Verification
Chạy `./verify.ps1` trên **starter đã sửa**. Hai chế độ đều in `bytes=128 shaMatch=True`, exit 0.

## Alternatives
Không hỗ trợ resume: tải lại toàn bộ với temp file và atomic rename, đơn giản nhưng tốn băng thông. Resume có ETag/If-Range và file temp: phù hợp production khi tài nguyên có thể thay đổi.

## Wrong / tempting fixes
Bỏ `EnsureSuccessStatusCode` không sửa nội dung. Cắt file cuối về 128 byte làm mất dữ liệu đúng và có thể che corruption. Retry vô hạn cùng offset lặp lại lỗi.

## Production implications / trade-offs
Bắt buộc xác thực ETag/If-Range, 416, Content-Length, short read, timeouts, atomic replace và hash toàn file. 206 không đủ nếu file trên server đổi version; dùng validator mạnh và kiểm tra checksum. Với nội dung có thể đổi, ưu tiên restart an toàn thay vì ghép hai version.

## Senior observation
Phân biệt transport success, protocol contract và end-to-end data integrity. Đặt invariant ở boundary trước khi mutate persistent file.
