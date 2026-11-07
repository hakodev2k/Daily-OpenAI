# Before — trạng thái starter

- `./reproduce.ps1` trả exit code 0 vì xác nhận được triệu chứng (`REPRO_FAIL` là marker của ứng dụng thử nghiệm, không phải script thất bại).
- `ON_DISK`: ba tệp `cal-101.json`, `cal-102.json`, `cal-103.json`.
- `ACCEPTED`: `CAL-101`, `CAL-103` (không có `CAL-102`).
- Callback trùng cho `CAL-103` không làm tăng bản ghi.
- `./verify.ps1` thất bại: còn thiếu `CAL-102` và `CAL-104`.
