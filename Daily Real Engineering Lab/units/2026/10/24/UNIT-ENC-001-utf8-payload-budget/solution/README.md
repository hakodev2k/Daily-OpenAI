# Reference Solution — chỉ mở sau khi tự reproduce và sửa

## 1. Symptoms
Nội dung ASCII hợp lệ chạy bình thường; một số message tiếng Việt hoặc emoji được local policy cho qua nhưng provider trả 413.

## 2. Evidence
Trong `run`, so sánh `utf16Units`, `wireBytes`, `localAllowed` và `providerStatus`. Ví dụ: chín ký tự `á` chiếm 9 UTF-16 code units nhưng 18 byte UTF-8; năm emoji `📦` chiếm 10 code units nhưng 20 byte.

## 3. Root cause
`string.Length` trong C# đếm UTF-16 code units, **không phải số byte khi encode UTF-8**. Starter dùng sai đơn vị để so với budget do provider công bố theo byte.

## 4. Why the fix works
`Encoding.UTF8.GetByteCount(message)` đo kích thước wire payload theo cùng encoding mà provider sử dụng. Policy và provider áp dụng cùng contract trước khi phát request. Giữ nguyên guards cho null và budget âm.

## 5. Verify
```powershell
./verify.ps1
dotnet run --project solution/EncodingLab.Reference.csproj -- verify
```
Lệnh thứ nhất xác nhận **starter đã được người học sửa**. Lệnh thứ hai chỉ kiểm tra reference để đối chiếu. Cả hai phải trả exit code 0; `reproduce.ps1` sẽ không còn trả 0 sau khi sửa thành công.

## 6. Alternatives
- Với payload nhỏ, `Encoding.UTF8.GetBytes(message).Length` cho kết quả tương đương nhưng cấp phát buffer không cần thiết.
- Với body gồm JSON envelope, chữ ký hoặc metadata, cần đo **toàn bộ serialized request bytes**, không chỉ phần message. Contract bài này giới hạn đúng UTF-8 byte của message.
- Nếu business yêu cầu cắt ngắn tự động, cần thuật toán duyệt Unicode scalar value (Rune) và không cắt giữa surrogate pair, kết hợp yêu cầu sản phẩm về truncation; không tự ý cắt trong bài này.

## 7. Wrong / tempting fixes
- Tăng budget cấu hình để che 413: làm mất tính nhất quán, không đảm bảo dữ liệu dài hơn.
- Retry 413: request không thay đổi, lỗi không phải transient.
- `Substring(0, maxPayloadBytes)`: vẫn đếm UTF-16 code units và có thể cắt giữa surrogate pair.
- Đổi `message.Length` sang số grapheme clusters: phù hợp một số giới hạn hiển thị, không phù hợp giới hạn byte.

## 8. Production implications
Ghi telemetry cho byte count, provider status, content-type và encoding; không log nội dung SMS nhạy cảm. Contract thực tế có thể giới hạn GSM-7/UCS-2 segments thay vì UTF-8; phải đọc tài liệu của **provider đang dùng** trước khi triển khai.

## 9. Trade-offs
Đo byte bằng `GetByteCount` có chi phí duyệt chuỗi O(n) nhưng tránh cấp phát toàn bộ encoded buffer. Validation phải nằm tại boundary gần serialization; nếu encoding hoặc payload format thay đổi, test contract phải thay đổi tương ứng.

## 10. Senior engineer should notice
Đơn vị đo là một phần của API contract. Test phải bao phủ exact boundary, over-boundary, Unicode supplementary characters và invalid input; không chỉ happy path ASCII. Một `413` không nên mặc định được retry hoặc gán cho network instability.
