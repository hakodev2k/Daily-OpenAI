# After — learner fix
- `verify.ps1` exit 0 với `VERIFIED=True`.
- `maxActive == 1` trong khi downstream bị giữ chậm.
- `activeAfterStop == 0`, không có batch mới bắt đầu sau `StopAsync`.
- Không được sửa harness hoặc biến check thành luôn-true; chỉ sửa implementation của runner.
