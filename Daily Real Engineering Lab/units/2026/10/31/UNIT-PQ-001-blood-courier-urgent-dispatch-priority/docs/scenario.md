# Nghiệp vụ và hợp đồng điều phối

Trung tâm máu tiếp nhận yêu cầu từ các bệnh viện. Mỗi yêu cầu có ID duy nhất và mức khẩn 0–9, số nhỏ hơn được xử lý trước. Trong thời gian chờ, bác sĩ có thể **nâng mức khẩn**; không có chức năng hạ mức khẩn trong lab này.

## Hợp đồng bắt buộc
- `Register(id, urgency)` tiếp nhận đúng một yêu cầu, ID phân biệt hoa/thường.
- `Escalate(id, newUrgency)` trả true khi mức mới khẩn hơn; trả false nếu không thay đổi, không tìm thấy hoặc đã điều phối.
- `DispatchNext()` trả ID có mức khẩn hiện hành cao nhất; cùng mức khẩn thì theo thứ tự tiếp nhận ban đầu.
- Mỗi yêu cầu được điều phối **tối đa một lần**; sau khi hết trả null.
- `PendingCount` phản ánh yêu cầu chưa điều phối, không phải số mục lưu trong cấu trúc nội bộ.
- Không hỗ trợ multi-thread trong phạm vi lab; tính an toàn đồng thời là bài mở rộng.

## Dữ liệu quan sát
- BLD-A: đăng ký mức 5, sau đó nâng lên 0.
- BLD-B: đăng ký mức 2.
- Kỳ vọng: BLD-A trước BLD-B.
- Các case còn lại kiểm tra nâng mức nhiều lần, trùng ID, FIFO và yêu cầu đã điều phối.

Không sửa các kỳ vọng nghiệp vụ để làm xanh bài test.
