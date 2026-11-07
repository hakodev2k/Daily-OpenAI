# After — Điều kiện chấp nhận

- verify.ps1 chạy trên **workspace/architecture.json** (không phải solution) và trả exit code 0 khi tất cả gate đạt.
- Peak memory mỗi instance <= 1536 MiB; completion <= 300 giây; compute cost <= 450 units/tháng.
- Mọi control bắt buộc được mô tả và đánh dấu, kèm rationale, failure recovery, trade-offs, ít nhất hai lựa chọn bị loại.
- Ví dụ direct-block-upload 1 instance: 128 MiB, ceil(32768/240)=137 giây, cost 150.
- Ví dụ streamed-proxy 2 instance: 192 MiB/instance, ceil(32768/160)=205 giây, cost 300.
- PASS chỉ chứng minh tính nhất quán **trong mô hình**. Phải tự kiểm tra assumptions về bandwidth, scan throughput, token leakage, storage cost, retention và concurrency thực.
