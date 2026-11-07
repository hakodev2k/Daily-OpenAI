# Before — starter chưa sửa

- `pwsh -File ./scripts/reproduce.ps1` trả `REPRODUCE_PASS`.
- `OUTBOUND_BAGGAGE` chứa `request_id` cùng các thuộc tính hành khách giả lập.
- `SENSITIVE_DATA_EXPORTED=True`, `CORRELATION_OK=True`, `BUSINESS_RESULT=ACCEPTED`.
- `pwsh -File ./scripts/verify.ps1` trả `VERIFY_FAIL` và exit code khác 0.
- Đây là mô phỏng local, không xác nhận hành vi collector/propagator production.
