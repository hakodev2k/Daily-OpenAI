# Hint 3 — Khái niệm

Trong `Range: bytes=3-6`, hai vị trí đầu/cuối đều thuộc đoạn được yêu cầu. API `AsMemory(start, length)` nhận **độ dài**, không nhận vị trí cuối. Kiểm tra công thức chuyển từ vị trí cuối sang độ dài và kiểm chứng cả `0-0`.
