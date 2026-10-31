# Dấu vết dự kiến (suy luận tĩnh, chưa thực thi)

Đây là output **dự kiến** từ starter chưa sửa, không phải log đã đo trong môi trường runtime:

~~~text
queued_cancelled=True
cancelled_work_executed=False
third_entered_before_first_released=True
first_fault=none
third_fault=SemaphoreFullException
healthy_queue_contract=True
cancel_after_acquire_contract=True
REPRODUCED: capacity invariant violated
~~~

Các nhánh trong Program.cs dùng TaskCompletionSource để điều khiển thứ tự. C không thể bị đánh dấu "early" do A hoàn thành vì harness chỉ gửi tín hiệu releaseFirst sau khi đọc trạng thái thirdEntered. Nếu kết quả khác, kiểm tra SDK, output lỗi, thay đổi code và đường dẫn project. Đừng suy ra hiệu năng từ một harness logic; chưa có benchmark GPU thật.
