# Hint 2 — Theo dõi vòng đời dữ liệu

Kiểm tra trạng thái nào tồn tại xuyên suốt nhiều lần gọi `BuildAsync`. Khi phương thức gặp `await` chưa hoàn thành, một lần gọi khác có thể tác động lên trạng thái đó không? So sánh dữ liệu đầu vào của mỗi lần gọi với nội dung nhãn sau khi tiếp tục.
