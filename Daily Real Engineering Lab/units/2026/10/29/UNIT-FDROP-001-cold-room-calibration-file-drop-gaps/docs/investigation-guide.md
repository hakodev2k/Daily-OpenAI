# Investigation guide — chỉ dùng sau khi chạy baseline

## Dữ liệu có thể thu thập
- `ON_DISK` và `ACCEPTED` trước/sau `Reconcile()`.
- Danh sách callback trong fixture và số lần xử lý thực tế.
- Kết quả `verify.ps1` khi có hồ sơ mới và callback lặp.

## Hypothesis cần kiểm chứng
1. Có tệp JSON không hợp lệ hoặc chưa ghi xong?
2. Logic lọc theo loại tệp/ID có loại nhầm?
3. Nguồn dữ liệu nào được coi là đủ để quyết định sổ kiểm định đã đầy đủ?

Đừng sửa bằng cách hard-code `CAL-102` hay sửa riêng fixture. Hãy thiết kế một invariant áp dụng cho mọi hồ sơ hoàn chỉnh trong thư mục.

## Định nghĩa hoàn thành
Mỗi chứng nhận hợp lệ trong snapshot thư mục được tiếp nhận đúng một lần; chạy đối soát nhiều lần và callback trùng không tạo thêm bản ghi. Ghi rõ những điều kiện mà demo chưa mô phỏng (file đang ghi dở, crash, multi-instance).
