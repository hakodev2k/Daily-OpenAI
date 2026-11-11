# Reference Solution — spoiler
Chỉ xem sau khi reproduce và thử fix.

## Symptoms & Evidence
Cache có 4 byte nhưng server trả 206 với `bytes 3-11/12`; append 9 byte thành 13 byte và lặp byte D.

## Root cause
Client kiểm tra HTTP status nhưng không kiểm tra phạm vi byte server thực sự trả về. Partial Content không có nghĩa là phạm vi phù hợp yêu cầu resume.

## Reference patch
Thay thân `ResumeAssembler.Append` bằng:

```csharp
if (response.StatusCode != HttpStatusCode.PartialContent)
    throw new InvalidDataException("Expected partial response");
var range = response.Content.Headers.ContentRange;
if (range is null || !range.HasLength || !range.HasRange ||
    range.From != existing.LongLength || range.To < range.From ||
    range.Length <= range.To ||
    range.To - range.From + 1 != incoming.LongLength)
    throw new InvalidDataException("Invalid resume range");
using var buffer = new MemoryStream();
buffer.Write(existing);
buffer.Write(incoming);
return buffer.ToArray();
```

## Why it works & verification
Không append khi offset khác độ dài đã có; test valid 4-11/12 pass, test 3-11/12 reject. Chạy `./scripts/verify.ps1` trên starter đã sửa, không phải solution độc lập.

## Alternative fixes
Nếu server bỏ qua Range và trả 200, có thể restart toàn bộ file sau khi xác minh version; không được append 200 vào prefix. Với tài nguyên thay đổi, dùng `If-Range` và ETag mạnh; thêm checksum toàn file.

## Wrong fixes
Cắt byte dư sau khi ghép có thể che lỗi nội dung. Retry cùng response sai không giúp. Chỉ kiểm tra `Content-Length` cũng không chứng minh đúng offset.

## Production implications & trade-offs
Kiểm tra range rẻ và fail-closed; cần quản lý phiên bản tài nguyên, xử lý 416, file lớn và lưu atomic. Senior engineer cần kiểm tra byte-range semantics, object identity và end-to-end integrity trước khi đánh dấu tải hoàn tất.
