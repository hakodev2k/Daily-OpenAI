# After — Acceptance Criteria

- `verify`: exit 0, có `PASS valid frame chunk=4096/1/2/3` và `VERIFY PASSED`.
- Truncated frame bị từ chối rõ ràng khi EOF, không giải mã dữ liệu thiếu.
- Frame length 2048 vượt giới hạn 1024 bị từ chối trước allocation.
- Cancellation lan truyền thành `OperationCanceledException`.
- `reproduce` không còn tái hiện lỗi gốc sau khi sửa; đây không phải regression.
