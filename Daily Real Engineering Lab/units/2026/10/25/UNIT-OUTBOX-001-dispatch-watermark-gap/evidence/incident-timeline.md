# Incident timeline — synthetic deterministic evidence

Đây là dữ liệu được tạo bởi acceptance harness, không phải production telemetry thật. **Chỉ đọc sau lần reproduce đầu tiên.**

| Step | Action | Observed |
|---|---|---|
| T0 | Outbox ready | sequence 101, 102, 103; checkpoint 0 |
| T1 | Publish 101 | ACK |
| T2 | Publish 102 | injected transient 503 (one-shot) |
| T3 | Publish 103 | ACK |
| T4 | Worker completes first batch | checkpoint 103 |
| T5 | Worker restarts next pass | query `Sequence > 103` returns empty |
| T6 | Partner reconciliation | event 102 missing; event 103 present |

Các số sequence và lỗi được cố định để reproduction ổn định. Quan sát độc lập qua `run.ps1` trước khi xem lời giải.
