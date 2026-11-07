# Before — starter ban đầu

- `./reproduce.ps1` thoát mã 0 và in `REPRODUCED`.
- `cold IDs=[101,103] correct=True`.
- `warmed IDs=[101,102,103] correct=False`.
- `./verify.ps1` thoát mã 1, in `FAIL`.
- Nếu chưa reproduce được, xác nhận project restore đúng EF Core SQLite 8.0.20, chưa copy solution vào starter, và SQLite connection được giữ mở trong suốt run.
