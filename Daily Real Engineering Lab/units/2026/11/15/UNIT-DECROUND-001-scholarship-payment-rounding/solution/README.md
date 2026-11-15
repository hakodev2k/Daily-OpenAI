# Reference Solution — chỉ đọc sau khi tự verify

## Symptoms
SV101 và SV102 có thể lệch một xu so với đối soát, dù SV104 khớp.

## Evidence
`1.05m * 0.5m = 0.525m`; `1.25m * 0.5m = 0.625m`. Các số này nằm đúng midpoint giữa hai giá trị có hai chữ số thập phân.

## Root cause
`Math.Round(decimal, 2)` mặc định sử dụng `MidpointRounding.ToEven` (banker's rounding), không giống chính sách hợp đồng `AwayFromZero`. Đây không phải lỗi precision của `decimal`.

## Why the fix works
Thay dòng `return Math.Round(raw, 2);` bằng `return Math.Round(raw, 2, MidpointRounding.AwayFromZero);`. Điều này quy định rõ hành vi tại midpoint.

## Verify
Chạy `./scripts/verify.ps1`. Kiểm tra cả bốn dòng và tổng 2.74. Thêm test cho 0.005m, 0.015m và giá trị không ở midpoint. Với khoản âm, cần hợp đồng riêng cho hoàn tiền.

## Alternatives
Một hàm Money policy tập trung giúp thống nhất toàn hệ thống, đặc biệt khi nhiều dịch vụ cùng tính toán.

## Wrong / tempting fixes
- Dùng `double`: tạo thêm rủi ro biểu diễn nhị phân, không giải quyết quy tắc hợp đồng.
- Cộng trước rồi làm tròn: thay đổi đối tượng được làm tròn và có thể lệch tổng từng khoản.
- Cộng 0.01 tùy trường hợp: phá vỡ các giá trị không ở midpoint.
- Sửa `Expected`: che lỗi đối soát thay vì sửa logic.

## Production implications
Cần thống nhất chính sách với đối tác, lưu số tiền đã chốt, theo dõi sai lệch từng khoản và kiểm thử boundary trước khi triển khai.

## Trade-offs
`AwayFromZero` không phải quy tắc đúng cho mọi sản phẩm. `ToEven` có thể phù hợp với các chính sách khác; cần version hóa quy tắc khi hợp đồng thay đổi.

## Senior engineer should notice
Không thể suy luận tính đúng chỉ từ tổng tiền; cần kiểm tra từng khoản, chính sách làm tròn, thứ tự làm tròn và đường đi dữ liệu tới hệ thống đối soát.
