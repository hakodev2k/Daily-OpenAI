# After — contract cần đạt

Sau khi sửa `starter/`:
- `reviewer` và `reviewer-with-auditor` trả `authorized=True`.
- `reader`, `no-role`, `near-match`, `unauthenticated` vẫn `False`.
- `./scripts/verify.ps1` trả exit code **0**; `reproduce.ps1` không còn xác nhận lỗi cũ.
- Không được thay `RequireRole` bằng policy cho phép tất cả, và không được coi claims từ HTTP header chưa xác thực là trusted.

Bài mở rộng: kiểm thử token hợp lệ nhưng thiếu role, nhiều role, và chuyển đổi claim từ nhiều identity provider.
