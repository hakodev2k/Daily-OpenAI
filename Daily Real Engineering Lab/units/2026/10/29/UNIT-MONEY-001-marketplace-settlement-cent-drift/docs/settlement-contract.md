# Hợp đồng nghiệp vụ settlement — phiên bản 1

Bảng giao dịch đầu vào có `SellerId`, `TransactionId`, `Amount`. `Amount` là decimal USD, dương khi thu và âm khi refund. `feeRate` là tỷ lệ phí đã thỏa thuận, từ 0 đến 1.

- Mỗi transaction có một khoản phí được hạch toán độc lập theo số tiền giao dịch của nó. Phí làm tròn đến cent theo `MidpointRounding.AwayFromZero`, kể cả khi giao dịch âm.
- Một seller statement là tổng `Gross`, `Fee`, `Net` của các giao dịch thuộc seller đó. `Net = Gross - Fee`.
- Báo cáo seller phải đối soát được với tổng phí của các chứng từ transaction; không được chuyển cent từ seller này sang seller khác.
- Không tự ý dùng `double` hoặc điều chỉnh chênh lệch bằng một khoản balancing không có chứng từ.
- Phạm vi lab: USD, hai chữ số thập phân, không xử lý nhiều currency hay thuế. TransactionId là dữ liệu đối soát; không triển khai deduplication trong lab này.

Acceptance: cả sales nhỏ, refunds, giao dịch thông thường và nhiều seller phải đúng, không chỉ tổng toàn batch.
