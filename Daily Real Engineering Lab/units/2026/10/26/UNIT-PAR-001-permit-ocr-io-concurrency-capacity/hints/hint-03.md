# Hint 03 — Parallel policy
Kiểm tra ParallelOptions.MaxDegreeOfParallelism. I/O-bound không nên mặc định bị giới hạn bởi logical CPU. Dùng giới hạn cấu hình nhưng không vượt capacity của đối tác.
