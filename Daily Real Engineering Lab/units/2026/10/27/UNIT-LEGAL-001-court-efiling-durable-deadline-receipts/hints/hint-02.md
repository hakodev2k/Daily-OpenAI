# Hint 02 — Boundary cần xác minh

Vẽ thứ tự: validate envelope tối thiểu → assign identity/time → storage commit → receipt response. Đặt một dấu X tại mọi điểm có thể mất điện hoặc timeout. Với retry, kiểm tra liệu cùng key và cùng payload có thể trả **chính cùng một** kết quả hay không.
