# Reference Solution — spoiler

## Symptoms và evidence
Hóa đơn nhỏ sai lệch vài phần trăm đơn vị tiền; starter có 3 trường hợp không khớp `contract.md`. So sánh các dòng trước/sau khi tổng hợp để xác định thời điểm sai lệch.

## Root cause
Starter cộng giá trị thô rồi mới gọi `Math.Round(rawTotal, 2)`. Hợp đồng yêu cầu làm tròn **từng dòng trước khi cộng**. Ngoài ra overload mặc định của `Math.Round` sử dụng `MidpointRounding.ToEven`, không phải chính sách `AwayFromZero`. Đây là hai nguyên nhân độc lập.

## Why fix works
`Math.Round(line.Quantity * line.UnitPrice, 2, MidpointRounding.AwayFromZero)` cho từng dòng, rồi cộng các kết quả đã ghi sổ. Credit âm được làm tròn đối xứng. Xem `Program.cs` cùng thư mục.

## Verification
Sao chép phần implementation vào `starter/Program.cs`, chạy `./verify.ps1`. Phải thấy `MISMATCHES=0`; thêm test hoán đổi thứ tự dòng và test ranh giới ±0.005, ±0.015.

## Alternative fixes
Có thể biểu diễn tiền bằng integer minor units sau khi làm tròn từng dòng, với chính sách chuyển đổi minh bạch. Cần kiểm tra overflow, scale và yêu cầu tiền tệ thực tế.

## Wrong / tempting fixes
- Chỉ đổi `ToEven` thành `AwayFromZero` trên tổng: vẫn sai thứ tự.
- Chỉ làm tròn từng dòng nhưng dùng overload mặc định: sai ở midpoint.
- Dùng `double`: đưa thêm sai số nhị phân không cần thiết.
- Chỉnh tổng bằng khoản chênh lệch thủ công: che giấu lỗi đối soát.

## Production implications & trade-offs
Đóng băng chính sách rounding theo version hợp đồng; lưu amount từng dòng, scale, currency và rounding mode; chạy reconciliation giữa ledger và invoice. Không mặc định 2 decimals cho mọi currency hoặc business contract. Senior engineer cần kiểm tra invariants, credit/refund symmetry, auditability và migration của hóa đơn lịch sử.
