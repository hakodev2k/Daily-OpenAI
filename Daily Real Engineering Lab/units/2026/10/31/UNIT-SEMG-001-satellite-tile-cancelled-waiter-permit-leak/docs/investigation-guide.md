# Investigation guide

Không đọc solution trước khi ghi kết quả.

1. Từ output, dựng timeline: A bắt đầu, B chờ, B bị hủy, C yêu cầu tài nguyên, A kết thúc, C kết thúc.
2. Chứng minh điểm đo third_entered_before_first_released là đồng bộ: harness kiểm tra trước khi gửi tín hiệu kết thúc cho A; không dựa vào sleep.
3. Phân biệt lỗi worker với lỗi cơ chế cấp quyền: callback của B có chạy không? Healthy queue có bị lỗi không?
4. Kiểm tra cả hai trường hợp hủy: hủy trước khi được cấp tài nguyên và hủy sau khi đã được cấp tài nguyên.
5. Đề xuất invariant: số tác vụ trong vùng giới hạn luôn nằm trong [0, 1]. Liệt kê các đường thoát khỏi hàm và đường nào thực sự có quyền giải phóng.
6. Sau khi sửa, ./reproduce.ps1 không còn phải pass; ./verify.ps1 phải pass. Không dùng việc nuốt exception làm tiêu chí hoàn thành.

Tự phân loại evidence: symptom, bằng chứng trực tiếp, bằng chứng phụ, giả thuyết chưa được xác nhận.
