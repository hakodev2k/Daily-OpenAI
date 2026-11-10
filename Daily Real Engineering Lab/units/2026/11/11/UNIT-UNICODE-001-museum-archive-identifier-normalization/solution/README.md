# Reference Solution

Hai chuỗi ID có hình thức tương tự nhưng code points khác nhau: U+00C9 và U+0045 U+0301. So sánh ordinal không tự áp dụng Unicode canonical equivalence. Chuẩn hóa `NormalizationForm.FormC` ở cả ingress và lookup, sau đó giữ `StringComparer.Ordinal`.

## Chạy
```powershell
dotnet run --project ./solution/MuseumRegistry.csproj -- --contract
./verify.ps1
```
Verify kiểm tra starter đã sửa, không kiểm tra solution.

## Wrong fixes
- `OrdinalIgnoreCase` chỉ xử lý case, không xử lý canonical equivalence.
- `ToLower()` không phải Unicode normalization, có rủi ro culture.
- Chỉ normalize lúc ghi thì lookup bằng input khác dạng vẫn lỗi.
- Xóa dấu làm gộp các ID khác nhau.

## Trade-offs
NFC khác NFKC; compatibility folding có thể phá domain identity. Dữ liệu đã lưu cần migration phát hiện collisions và quy tắc merge/audit; code mẫu dùng last-write-wins chỉ để mô phỏng. Không normalize tùy tiện các giá trị cần byte-exact như chữ ký mật mã.
