# Before — hành vi starter

- `reproduce` exit code 0 khi tái hiện được triệu chứng, không đồng nghĩa mọi test pass.
- Các case có header biểu diễn thời điểm có thể báo `FAIL` dù response có status retryable.
- Nhóm delta-seconds và non-retryable status vẫn có thể PASS.
- `verify` exit code 3, summary không phải 13/13.
