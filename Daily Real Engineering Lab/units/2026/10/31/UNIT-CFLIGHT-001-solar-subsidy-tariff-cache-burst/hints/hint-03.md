# Hint 03 — Cơ chế cần cân nhắc

Nghiên cứu per-key single-flight: nhiều request cùng key có thể chờ chung một Task đang chạy. Nếu dùng ConcurrentDictionary.GetOrAdd, factory có thể được gọi nhiều lần; cân nhắc trì hoãn side effect và dọn trạng thái inflight cả khi upstream lỗi.
