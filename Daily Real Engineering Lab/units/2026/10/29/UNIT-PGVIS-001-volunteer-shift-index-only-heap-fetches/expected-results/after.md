# After
- `./verify.ps1` dựng lại fixture lỗi rồi chạy **workspace/remediation.sql của learner**.
- Index Only Scan, Heap Fetches = 0; Actual Rows = Count = 5.000.
- Không dùng ngưỡng milliseconds cố định; số buffer hits phụ thuộc máy.
- Giải pháp production phải tính đến churn, vacuum cadence, I/O, WAL, bloat và monitoring.
