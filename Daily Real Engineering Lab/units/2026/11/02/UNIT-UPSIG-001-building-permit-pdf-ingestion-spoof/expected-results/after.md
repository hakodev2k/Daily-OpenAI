# After — learner workspace

- `pwsh ./scripts/verify.ps1` trên `starter/` trả exit code 0 và `VERIFIED`.
- Tệp mẫu hợp lệ vẫn được chấp nhận; payload giả dạng, header ngắn/gần giống, extension/media type không phù hợp, empty và quá cỡ đều bị từ chối.
- `reproduce.ps1` vẫn tái hiện baseline bất kể learner đã sửa starter.
- Signature gate chỉ là lớp sàng lọc ban đầu, không thay thế parser thực tế, AV scan, sandbox, giới hạn tài nguyên hoặc chính sách lưu trữ.
