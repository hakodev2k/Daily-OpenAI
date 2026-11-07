# Before — Starter

- `./reproduce.ps1` trả mã 0 khi nhận diện lỗi gốc.
- Fixture có 5 cửa sổ đến hạn nhưng chỉ 1 cửa sổ được ghi nhận sau wake-up.
- Checkpoint dừng ở 01:00 thay vì 05:00; không có exception.
- `./verify.ps1` phải **FAIL** trên starter chưa sửa (thường là `Expected 5 completed windows, got 1.`).
- Nếu máy quá tải khiến demo timer lệch thời gian, dựa vào fixture logical clock 5 giờ để tái hiện; không kiểm thử bằng ngưỡng millisecond chính xác.