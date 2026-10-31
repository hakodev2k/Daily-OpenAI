# After — Learner fix

- `verify.ps1` trả exit code 0.
- Cả ba case in `ACCEPT`, `reason=accepted`.
- `sentBytes == declaredBytes` ở mỗi case, frame giải mã được đúng sensor ID và nhiệt độ.
- Negative case (truncated frame) vẫn bị từ chối.
- `reproduce.ps1` không còn chứng minh lỗi ban đầu sau khi starter đã được sửa; đó là hành vi mong đợi.
- Kiểm tra trade-off: copy byte array dễ quản lý ownership nhưng có chi phí allocation; zero-copy cần truyền slice có offset/count và bảo đảm lifetime.
