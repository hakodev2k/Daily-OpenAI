# Hint 3 — Concept
Giao thức yêu cầu chính xác 4 byte header rồi N byte payload. Xem hợp đồng đọc đủ N byte (hoặc vòng lặp offset có kiểm tra EOF) trong API Stream của .NET.
