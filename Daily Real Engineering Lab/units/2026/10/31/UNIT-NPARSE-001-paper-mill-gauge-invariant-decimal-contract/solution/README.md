# Reference Solution — SPOILER

Chỉ xem sau khi reproduce và ghi giả thuyết.

## 1. Symptoms
File giống nhau nhưng `en-US` đọc đúng, `de-DE` có thể hiểu dấu chấm thành phân nhóm, locale khác có thể báo lỗi. Token có dấu phẩy có thể được nhận dù vi phạm hợp đồng.

## 2. Evidence
`starter/Program.cs` chạy fixture qua bốn locale, so sánh giá trị `decimal` chính xác và kiểm tra input bất hợp lệ.

## 3. Root cause
`CultureInfo.CurrentCulture` và `NumberStyles.Number` làm parser phụ thuộc cấu hình máy; style này còn cho phép dấu nhóm. Wire format cần nghĩa cố định.

## 4. Why the fix works
Thay `starter/GaugeImport.cs` bằng [reference source](GaugeImport.cs). Dùng `CultureInfo.InvariantCulture`, style giới hạn `AllowLeadingSign | AllowDecimalPoint`, `decimal.TryParse` và từ chối token không hợp lệ.

## 5. How to verify
```powershell
./scripts/verify.ps1
```
Kỳ vọng 9 PASS, 0 FAIL, exit 0. `reproduce.ps1` sau sửa sẽ exit 1 vì lỗi gốc không còn.

## 6. Alternative fixes
`decimal.Parse` với invariant culture và restricted style cũng hợp lệ nếu muốn exception. Có thể dùng parser theo schema rõ ràng cho file phức tạp hơn.

## 7. Wrong / tempting fixes
- `Replace(".", ",")`: có thể thay đổi nghĩa dữ liệu.
- Ép `CurrentCulture` toàn tiến trình sang en-US: gây side effect và không phải contract tại boundary.
- `NumberStyles.Any`: nhận cú pháp không thuộc wire contract.
- Đổi sang `double`: không xử lý ambiguity và có rủi ro precision.

## 8. Production implications
Tách wire parsing khỏi localized UI. Log raw token/site/parser version có kiểm soát, quarantine file sai, không âm thầm sửa dữ liệu.

## 9. Trade-offs
Strict parser có thể từ chối file cũ từng được nhận nhầm. Cần thống kê compatibility, migration và agreement với partner.

## 10. What a Senior engineer should notice
Silent numeric corruption nguy hiểm hơn parse failure. Test cross-culture và negative input là regression bắt buộc; không lấy output của một locale làm oracle cho locale khác.
