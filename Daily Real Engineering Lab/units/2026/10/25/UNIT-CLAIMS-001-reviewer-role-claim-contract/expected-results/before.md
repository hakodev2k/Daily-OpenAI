# Before — trạng thái starter

Chạy `./scripts/reproduce.ps1` trước khi sửa:
- `reviewer` và `reviewer-with-auditor` trả `authorized=False` dù token đã được xác thực.
- `reader`, `no-role`, `near-match`, `unauthenticated` đều `False`.
- Script trả exit code **0** khi tái hiện được đúng triệu chứng.
- `./scripts/verify.ps1` trả exit code **1** vì contract tích cực chưa thỏa mãn.

Nếu không reproduce: kiểm tra SDK 8, `dotnet restore`, đảm bảo đang dùng bản starter chưa sửa và chạy lệnh từ thư mục unit.
