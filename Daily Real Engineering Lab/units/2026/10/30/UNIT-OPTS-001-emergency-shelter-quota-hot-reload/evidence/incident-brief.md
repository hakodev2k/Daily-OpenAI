# Incident brief — dữ liệu mô phỏng, không phải production log
- 08:00: điều phối viên cấu hình MaxBedsPerRequest=4; service khởi tạo.
- 08:10: quota giảm còn 2 khi sức chứa còn thấp; UI config hiển thị 2.
- 08:11: một yêu cầu 3 giường vẫn được worker chấp nhận.
- 08:20: quota tăng lên 5 sau khi mở khu nhà mới.
- 08:21: yêu cầu 5 giường bị từ chối dù dashboard hiển thị 5.
- Không ghi nhận process restart giữa các mốc.
Cần thu thập bằng chứng thực thi từ `reproduce.ps1`; timeline này chỉ cung cấp giả thuyết ban đầu.
