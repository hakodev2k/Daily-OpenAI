# Senior Challenge — ambiguous and invalid local appointments

Product thay đổi yêu cầu: một loại phỏng vấn có thể bắt đầu lúc **02:30**. Vào ngày chuyển mùa, giờ địa phương này có thể không tồn tại hoặc xuất hiện hai lần.

1. Viết policy rõ ràng cho nonexistent time (reject, move forward, hoặc hỏi người đặt lịch) và ambiguous time (chọn offset nào, hoặc yêu cầu explicit offset).
2. Thiết kế dữ liệu lưu: local date, local time, IANA zone ID, resolved UTC instant, timezone-rule version khi audit quan trọng.
3. Bổ sung regression checks cho ngày chuyển mùa mà không phụ thuộc vào `DateTime.Now` hoặc timezone máy developer.
4. Phân tích tác động khi timezone database được cập nhật giữa lúc tạo lịch và lúc thực thi.

Không coi `DateTimeOffset` đơn lẻ là thay thế hoàn toàn cho timezone ID của lịch lặp.
