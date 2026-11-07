# After — Hợp đồng đã sửa

- verify.ps1 in VERIFY_PASS sau khi chạy chính starter/ đã sửa.
- 128, 131072, 524288 ký tự stderr đều hoàn tất exit 0 và STREAMS_DRAINED: bytes=N.
- Child trả exit 7 được phân loại CHILD_FAILED: exit=7 và wrapper exit 31.
- Timeout vẫn dành cho child thực sự treo; process tree được dọn khi hết hạn.
- Không dùng ngưỡng latency tuyệt đối để so sánh máy khác nhau.
