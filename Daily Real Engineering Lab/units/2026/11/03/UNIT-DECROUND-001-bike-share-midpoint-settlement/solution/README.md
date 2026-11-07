# Reference Solution — chỉ đọc sau khi thử

## Symptoms
Một số chuyến có phí nội bộ thấp hơn bảng kê một cent dù dữ liệu gốc chính xác.

## Evidence
Các giá trị 1.005, 2.015, 3.125 là midpoint tại precision 2; 4.234 không phải midpoint.

## Root cause
`Math.Round(decimal, 2)` mặc định `MidpointRounding.ToEven`; đối tác quy định `AwayFromZero` trên mỗi chuyến. Vì vậy 1.005 -> 1.00 thay vì 1.01.

## Why fix works
Dùng `Math.Round(fee, 2, MidpointRounding.AwayFromZero)` trong `Settlement.Charge` (xem source reference), rồi cộng các khoản đã chốt.

## Verify
Chạy `./verify.ps1` trên starter đã sửa; 4 khoản khớp và tổng 10.39.

## Alternatives
Policy object khi nhiều đối tác có quy tắc khác nhau; integer minor units sau bước chốt phí, không thay thế rounding policy.

## Wrong fixes
Đổi sang double làm tăng rủi ro sai số; làm tròn tổng thay vì từng chuyến vi phạm hợp đồng; bỏ assert hay thêm tolerance che sai lệch thực.

## Production implications
Lưu policy version, raw amount, settled amount để audit. Cần test zero, negative/refund và currency precision; thay policy không được hồi tố vào giao dịch đã khóa.

## Trade-offs / Senior insight
Quy tắc làm tròn là business contract, không phải style; phải version hóa và kiểm chứng với đối tác trước rollout.
