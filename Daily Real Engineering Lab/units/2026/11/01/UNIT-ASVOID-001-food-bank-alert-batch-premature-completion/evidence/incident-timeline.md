# Evidence — Timeline mô phỏng

| Mốc | Batch state | Gateway acknowledged | Ghi chú |
|---|---|---:|---|
| T0 | STARTED | 0/3 | Worker lấy ba cảnh báo nhiệt độ |
| T1 | COMPLETED | 0/3 | Hàm dispatch trả về |
| T2 | COMPLETED | 1/3 | Gateway bắt đầu hoàn tất |
| T3 | COMPLETED | 3/3 | Cả ba send hoàn tất |

Dữ liệu được tạo bởi harness có cổng chờ, không phải log production thật. Chỉ dựa vào timeline chưa thể kết luận lỗi ở worker, gateway hay telemetry. Đối chiếu với kết quả `reproduce.ps1` và code trước khi đưa ra root cause.
