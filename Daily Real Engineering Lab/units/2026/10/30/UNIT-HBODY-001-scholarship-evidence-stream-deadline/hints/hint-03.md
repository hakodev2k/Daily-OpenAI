# Hint 3 — Concept
Cần một cancellation budget chung tồn tại xuyên suốt cả `SendAsync` và body read. Cân nhắc liên kết token của caller với budget, rồi chuyển cùng token đến API đọc body có overload nhận `CancellationToken`.
