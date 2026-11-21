# Reference solution — chỉ mở sau khi verify

Hợp đồng POS quy định thuế được làm tròn **theo từng dòng**, rồi mới cộng. Starter cộng các giá trị thuế chưa làm tròn và làm tròn một lần ở cấp giỏ hàng. Với 3 dòng giá 0.05 và rate 10%, mỗi dòng có thuế 0.005: làm tròn từng dòng thành 0.01, tổng 0.03; starter cho 0.02.

Reference code đặt `decimal.Round(line.Net * line.Rate, 2, MidpointRounding.AwayFromZero)` bên trong phép `Sum`. Giá trị âm cho refund dùng cùng chính sách midpoint để giữ tính đối xứng.

## Vì sao những fix khác sai
- Dùng `double`: thêm sai số biểu diễn nhị phân, không giải quyết vị trí làm tròn.
- Đổi sang `MidpointRounding.ToEven` ở cấp giỏ: thay đổi chính sách nhưng vẫn không tuân thủ ranh giới từng dòng.
- Cộng thêm 0.01 cho giỏ nhỏ: vá dữ liệu cụ thể, hỏng khi số dòng và dấu thay đổi.
- Chỉ sửa expected của test: che giấu sai lệch hợp đồng POS.

## Trade-offs
Trong production cần phiên bản hóa rounding policy, xác định currency minor units (không phải mọi tiền tệ có 2 chữ số), quy tắc discount và refund, cùng chiến lược migration/reconciliation cho hóa đơn đã phát hành. Không tự thay đổi hóa đơn lịch sử.
