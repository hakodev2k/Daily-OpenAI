# Incident evidence — synthetic, reproducible fixture

- 2026-10-28 07:15 ICT: batch `brake-repair-routing` báo `candidateCount=5`.
- Field audit báo `confirmedBrakeFailures=3`; đội sửa chữa đã mở hai ticket không cần thiết.
- MongoDB `turbines` chứa 6 documents, mỗi document có mảng `inspections`.
- `WT-041`: brake=PASS, blade=FAILED; xuất hiện trong danh sách routing.
- `WT-042`: brake=FAILED, blade=PASS; xuất hiện trong danh sách routing.
- `WT-046`: brake=PASS, electrical=FAILED; xuất hiện trong danh sách routing.
- Log phụ: `workerRestartCount=0`, `mongoTimeoutCount=0`; chưa có bằng chứng worker restart hay timeout gây lỗi.

Các log trên là **synthetic fixture**, không phải telemetry từ production thật. Kiểm chứng bằng `./run.ps1` và đọc `starter/query.js` trước khi xem hints.
