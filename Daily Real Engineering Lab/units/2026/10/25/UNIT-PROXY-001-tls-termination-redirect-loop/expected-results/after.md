# After — learner fix

- `verify.ps1` trả exit code 0 trên `starter/` đã sửa.
- `EDGE_SIM` trả 200, JSON có `scheme: "https"`.
- `DIRECT_HTTP` vẫn trả 307, `Location` dùng HTTPS.
- Không được bật tin cậy `X-Forwarded-Proto` từ mọi địa chỉ; production phải giới hạn trusted proxy và cấu hình ingress đúng.
- `verify.ps1 -Reference` là phép đối chiếu tùy chọn, không thay thế kiểm tra code của người học.
