# Incident evidence — synthetic, deterministic

> Dữ liệu dưới đây là mô phỏng để luyện investigation; không phải production telemetry thực.

- 10:02:15 — Gateway báo `GRID-NORTH-11` đã trả công suất 42 kW.
- 10:02:15 — API dispatcher ghi `InvalidOperationException` trước khi trả receipt.
- 10:02:15 — Audit event count: 0, mặc dù gateway request count: 1.
- 10:02:16 — Worker CPU và latency gateway bình thường; không thấy timeout hoặc retry.
- 10:05:00 — Triệu chứng vẫn xuất hiện với cùng signal trong môi trường local.

## Triage
Đừng kết luận lỗi từ tên exception. Phân tách các mốc: lấy reading, xử lý reading, ghi audit, trả receipt. Đối chiếu code với hợp đồng của kiểu giá trị bất đồng bộ được trả về.
