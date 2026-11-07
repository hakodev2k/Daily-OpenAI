# Evidence — deterministic incident timeline

| Signal | Slow partner | Fast partner | Failed partner |
|---|---|---|---|
| Grace period | Before snapshot | After snapshot | Before exception |
| Snapshot | After first read | Before first read | Never available |
| Starter result | Empty without exception | Valid | Empty without exception |

CPU spikes, retry storm và cache key mismatch chỉ là giả thuyết gây nhiễu; harness không có bằng chứng cho chúng.
