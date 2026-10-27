# Before — Starter
- setup.ps1 build thành công khi có .NET 8 SDK/runtime phù hợp.
- reproduce.ps1 trả exit code 0 nếu quan sát được ít nhất một correlation mismatch ở parallel và không có mismatch ở single-tile.
- verify.ps1 trả exit code 1 khi còn mismatch.
- Mỗi batch vẫn trả đủ số rows; giá trị audit cuối cùng không cần cố định.
- Nếu không tái hiện: kiểm tra đang chạy đúng project starter và chưa thay đổi TileAuditPipeline.
