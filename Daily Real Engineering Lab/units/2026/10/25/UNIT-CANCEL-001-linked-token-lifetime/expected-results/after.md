# After — learner đã sửa starter

Chạy `verify.ps1`:
- Hai dòng `caller` và `host`: `scannerCalled=True`, `downstreamTokenCancelled=True`, `taskCancelled=True`, `pendingBeforeRelease=False`.
- Test normal completion trả `ScanReceipt` đúng và không hủy sớm.
- Dòng cuối: `VERIFIED`; exit code 0.
- `verify.ps1 -Reference` kiểm tra riêng reference source, không dùng để thay thế kiểm thử starter.

Tốc độ máy có thể khác nhau; kiểm thử dùng bounded wait để tránh treo vô hạn, không yêu cầu ngưỡng latency production.
