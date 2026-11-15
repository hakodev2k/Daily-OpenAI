# Hợp đồng thanh toán

Hệ thống gửi số tiền theo đơn vị có **hai chữ số thập phân**. Đơn vị thanh toán quy định khi chữ số bị loại bỏ nằm chính xác tại nửa đơn vị nhỏ nhất thì làm tròn **ra xa 0**. Mỗi khoản được làm tròn trước khi cộng tổng. Không được làm tròn tổng của các số chưa làm tròn rồi phân bổ lại.

Đầu vào đã được duyệt, không có âm và dùng `decimal`. Không được thay đổi `Expected` để khiến bài kiểm tra qua. Đánh giá các trường hợp không nằm tại điểm giữa và những khoản bằng 0.

Đầu ra đối soát kỳ vọng: SV101=0.53; SV102=0.63; SV103=0.58; SV104=1.00; tổng=2.74.
