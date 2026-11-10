# Reference Solution (spoiler)

Thay `BoundedChannelFullMode.DropOldest` bằng `BoundedChannelFullMode.Wait` để `WriteAsync` chỉ hoàn thành khi còn chỗ. Với sink bị giữ lại và capacity=2, producer phải chờ; sau khi sink được mở, cả bốn ID được giao đúng thứ tự. Không dùng unbounded channel như một fix mặc định: nó chuyển nguy cơ mất dữ liệu thành nguy cơ OOM. Trong production cần thêm cancellation, timeout policy, metrics queue depth, và lựa chọn durable queue nếu yêu cầu chống mất dữ liệu khi process crash.

Chạy `./verify.ps1` từ thư mục lab sau khi tự sửa `starter/`. Reference ở đây chỉ để đối chiếu, không được verify thay cho bài làm.
