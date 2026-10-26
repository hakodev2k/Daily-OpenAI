# Reference Solution — chỉ xem sau khi tự investigate

## 1. Symptoms

Các fixture nhiều chunk có kết quả dài hơn nguồn và chứa byte lặp tại ranh giới; fixture 1 byte vẫn đúng. HTTP 206 không đảm bảo client ghép đúng.

## 2. Evidence

Trace 10 byte: `0-4` (5 byte), `4-8` (5 byte), `8-9` (2 byte), tổng 12 byte. Byte ở offsets 4 và 8 xuất hiện hai lần.

## 3. Root cause

HTTP byte range theo RFC 9110 dùng **inclusive first-byte-pos và last-byte-pos**. Starter tính `end = start + chunkSize` (rồi clamp) như thể đầu cuối exclusive. Mỗi chunk đầy đủ nhận thêm một byte và chunk tiếp theo lại bắt đầu tại byte đó.

## 4. Why the fix works

Tính `end = min(totalLength - 1, start + chunkSize - 1)`. Với chunk 4 byte: `0-3`, `4-7`, `8-9`. Mỗi byte xuất hiện đúng một lần. [Reference source](ChunkDownloader.cs) cũng kiểm tra `Content-Range` và payload length để tránh append response sai.

## 5. How to verify

Chỉnh file trong `starter/`, chạy `.\verify.ps1`. Kiểm tra 10/12/1 byte đều bằng nguồn; chạy `.\reproduce.ps1` trên starter nguyên bản trước khi sửa. Reference code không được gọi trực tiếp bởi verify.

## 6. Alternative fixes

Có thể tính số byte cần tải bằng `count = min(chunkSize, totalLength-start)` rồi `end = start+count-1`, hoặc dùng streaming với strict offset bookkeeping. Một HTTP GET toàn bộ file là hợp lý khi không cần resume, nhưng không đáp ứng nhu cầu tải tiếp.

## 7. Wrong / tempting fixes

- Cắt byte thừa ở cuối file: không loại được các byte chồng lấn nằm giữa các chunk.
- Chỉ tin `206 Partial Content`: server có thể trả response hợp lệ cho một request sai.
- Giảm chunk size: thay đổi tần suất lỗi, không sửa semantics.
- Chỉ so sánh tổng length: hai payload có thể cùng length nhưng khác nội dung.

## 8. Production implications

Khi tải file thật, kiểm tra `Content-Range`, ETag hoặc If-Range để tránh ghép các version khác nhau, retry an toàn cho từng range, giới hạn bộ nhớ và xác minh checksum sau cùng. Server có thể bỏ qua Range và trả 200; phải xử lý rõ ràng, không append mù.

## 9. Trade-offs

Nhiều range request cho phép resume và bounded transfer, nhưng tăng overhead và yêu cầu nhất quán version. Một GET giảm độ phức tạp nếu kích thước nhỏ. Streaming tránh `MemoryStream` lớn nhưng cần quản lý output file và partial writes.

## 10. Senior engineer should notice

Đừng kết luận lỗi mạng khi tất cả status đều 206. Phân biệt wire contract, byte-offset arithmetic, object version consistency và final integrity. Tạo regression ở boundary: đúng bội số chunk, không chia hết, file một byte, server trả sai `Content-Range` hoặc đổi ETag.

## Primary reference

RFC 9110 §14.1.2 Byte Ranges: https://www.rfc-editor.org/rfc/rfc9110.html#section-14.1.2
