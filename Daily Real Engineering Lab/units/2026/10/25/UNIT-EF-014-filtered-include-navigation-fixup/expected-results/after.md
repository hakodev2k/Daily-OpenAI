# After — code người học đã sửa

- `./verify.ps1` thoát mã 0.
- `cold IDs=[101,103] correct=True`.
- `warmed IDs=[101,103] correct=True`.
- `missing-cohort contract: PASS` và `PASS: cold/warmed roster and missing-cohort contract`.
- Không cần so thời gian chạy chính xác giữa các máy. Dữ liệu seed và thứ tự ID là deterministic.
- Sau fix, `./reproduce.ps1` thoát mã 1 là bình thường: triệu chứng cũ không còn.
