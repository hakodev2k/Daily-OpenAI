# Reference Solution — chỉ xem sau khi đã điều tra

## Triệu chứng
Tổng thuế đã in trên hóa đơn là 0.03 nhưng số chuyển ledger là 0.02.

## Evidence
Mỗi dòng có Net=0.05, Rate=10%, thuế thô=0.005. Theo `AwayFromZero`, thuế mỗi dòng = 0.01. Tổng ba dòng = 0.03.

## Root cause
Hai luồng dùng **hai cấp làm tròn khác nhau**: invoice làm tròn từng dòng, ledger cộng các giá trị chưa làm tròn rồi làm tròn một lần. Phép làm tròn không phân phối qua phép cộng.

## Why fix works
Ledger tính `lines.Sum(LineTax)` để dùng cùng một chính sách đã xác nhận với hóa đơn; không còn hai thuật toán tính thuế.

## How to verify
Sửa `starter/Program.cs` rồi chạy `./verify.ps1`. `verify.ps1` chạy **starter đã chỉnh sửa**, không chạy solution. Có thể so sánh bằng `dotnet run --project solution/TheaterTaxLab.csproj -- --verify`.

## Alternative fixes
Nếu hợp đồng pháp lý quy định làm tròn ở cấp hóa đơn thay vì từng dòng, phải thay đổi **cả hai** đầu ra và tài liệu hợp đồng, đồng thời kiểm thử các hóa đơn lịch sử. Không tự ý đổi quy tắc.

## Wrong / tempting fixes
Không dùng `double` để che chênh lệch; không cộng một khoản điều chỉnh `0.01` cố định; không tăng số chữ số hiển thị; không bỏ qua mismatch dưới ngưỡng mà không có chính sách.

## Production implications
Lưu tax policy version, tax base, rate, line-level tax, invoice total và ledger posting ID; reconcile theo invoice ID; test refund, quantity, nhiều thuế suất, tiền tệ, số âm, tax-inclusive price. Cần xác nhận yêu cầu pháp lý theo từng thị trường.

## Trade-offs
Line-level rounding khớp chứng từ từng dòng nhưng có thể khác tổng raw-tax. Invoice-level rounding có thể phù hợp với một số quy định khác. Quy tắc phải nhất quán và versioned.

## Senior engineer should notice
Vấn đề là **contract boundary** giữa chứng từ khách hàng và accounting, không phải lỗi hiển thị. Cần invariant, audit trail, replay và migration plan trước khi sửa các giao dịch đã chốt.
