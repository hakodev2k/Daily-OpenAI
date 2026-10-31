# Hint 03 — Hướng sửa

Mỗi deferred action cần có một payload ổn định thuộc riêng nó. Cân nhắc tạo command bất biến ở phạm vi mỗi iteration trước khi tạo delegate; hoặc thay queue nhận `Action` bằng queue nhận `PatrolNotice` và worker xử lý dữ liệu. Giữ oracle không đổi.
