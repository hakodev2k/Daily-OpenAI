# Hint 03 — Hướng giải pháp

Xem xét đặt điều kiện tối thiểu số người trực và thay đổi trạng thái vào **cùng một atomic write boundary**. Với database thật, cần chọn transaction isolation, constraint hoặc cơ chế serialize/CAS thích hợp; một lock trong process không đủ khi scale-out.
