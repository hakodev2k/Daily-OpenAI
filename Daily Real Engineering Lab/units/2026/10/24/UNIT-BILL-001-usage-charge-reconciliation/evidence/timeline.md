# Incident timeline (mô phỏng)
- 09:00 triển khai billing service; API health không đổi.
- 09:15 settlement ledger phát hiện 5/8 invoice lệch.
- 09:30 retry import không đổi event count/checksum.
- 09:40 có invoice credit âm, có invoice không; các charge chính xác đến cent thường khớp.
- 10:00 on-call yêu cầu không sửa raw audit data.

Giả thuyết nhiễu: duplicate events, trễ đồng bộ, culture, precision.
