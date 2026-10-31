# Hint 3 — Khái niệm cần kiểm tra

Trong SemaphoreSlim, chỉ một lời gọi WaitAsync hoàn tất thành công mới sở hữu permit. Khối finally phải giải phóng **có điều kiện theo acquisition**, kể cả khi callback render ném exception hoặc cancellation sau đó. Không được giải phóng khi WaitAsync chưa thành công.
