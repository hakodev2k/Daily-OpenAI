# Trước khi sửa
- `scripts/setup.ps1` cần restore/build thành công trên .NET 8 SDK.
- `scripts/run.ps1` in ra trạng thái W-17 đổi từ OPEN → CLOSED → SUSPENDED khi backlog được replay.
- `scripts/reproduce.ps1` trả exit code 0 và dòng `REPRODUCED expected starter violations`, đồng thời có các dòng FAIL cho contract bị vi phạm.
- `scripts/verify.ps1` trên starter gốc phải trả exit code khác 0.
Nếu không tái hiện, kiểm tra đang chạy đúng thư mục unit và chưa thay đổi starter.
