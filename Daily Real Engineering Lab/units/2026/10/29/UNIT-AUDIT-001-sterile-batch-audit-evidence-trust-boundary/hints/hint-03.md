# Hint 03 — Cơ chế integrity

Thử mô hình mỗi event có canonical representation và liên kết theo thứ tự trong phạm vi một batch. Tách việc lưu dữ liệu, chốt checkpoint và xác minh bằng các quyền độc lập. Thiết kế phải phát hiện cả thay nội dung **không đổi count**, và nói rõ trường hợp signing key bị chiếm.
