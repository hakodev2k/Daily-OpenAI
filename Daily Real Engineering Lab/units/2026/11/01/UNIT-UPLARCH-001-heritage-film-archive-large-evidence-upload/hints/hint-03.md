# Hint 3 — Capacity và lifecycle

Trong scripts/evaluate.ps1, mô hình thời gian = tổng MiB / throughput tổng; memory phụ thuộc số upload đang được xử lý trên mỗi instance. Một phương án tốt phải có finalize idempotent, kiểm tra checksum, scan gate và cleanup, không chỉ đáp ứng SLO.
