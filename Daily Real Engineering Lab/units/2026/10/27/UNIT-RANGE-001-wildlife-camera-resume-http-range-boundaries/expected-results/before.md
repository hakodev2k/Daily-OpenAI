# Before — starter

- Fixture 10 byte: `downloadedBytes=12`, `sameBytes=False`.
- Fixture 12 byte: `downloadedBytes=14`, `sameBytes=False`.
- Fixture 1 byte: `downloadedBytes=1`, `sameBytes=True`.
- Tất cả request được server fixture chấp nhận với 206; `reproduce.ps1` phải exit 0 khi đúng hai fixture lỗi.
- Nếu không tái hiện: chạy `dotnet --version`, kiểm tra SDK 8.x, và chạy `run.ps1` từ thư mục unit.
