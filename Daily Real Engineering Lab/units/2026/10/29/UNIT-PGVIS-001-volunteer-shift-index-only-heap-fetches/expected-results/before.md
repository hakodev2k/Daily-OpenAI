# Before
- `./reproduce.ps1` exit 0 khi **chứng minh lỗi** (REPRODUCED).
- Baseline sau initial VACUUM: Index Only Scan, Heap Fetches = 0.
- Sau metadata UPDATE: Index Only Scan, Heap Fetches > 0.
- Count = 5.000; cùng truy vấn, cùng covering index.
- `./verify.ps1` với remediation chỉ có comment phải thất bại.
