# Investigation guide
Chạy `./reproduce.ps1`, lưu lại stdout và ghi evidence vào `workspace/my-investigation.md`.

- Worker có tiến triển không nếu gateway từ chối một message?
- Một lỗi của site A có làm mất cơ hội xử lý của site B không?
- Điều gì thay đổi nếu gateway chỉ lỗi tạm thời đúng một lần?
- Những điều kiện nào buộc phải giữ thứ tự trong cùng site?
- Cần đo metrics nào: attempts, delivery latency, retry backlog, dead-letter depth, age of oldest pending?
- Lập bảng quyết định: permanent, transient, unknown; hành động, giới hạn, observability và operational owner.

Chỉ sau khi có hypothesis mới mở hints. Sau khi sửa chạy `./verify.ps1`; giữ bằng chứng cả ba assertions.
