# Contract và evidence

## Starter
- Trace cho thấy CRITICAL được cập nhật severity=1 nhưng ROUTINE vẫn dispatch trước.
- `reproduce.ps1` kết thúc exit 0 khi triệu chứng tái hiện.
- `verify.ps1` báo ít nhất một FAIL và exit 1.

## Sau khi sửa
- CRITICAL severity=1 được dispatch trước ROUTINE severity=4.
- Hạ mức ưu tiên của A từ 1 xuống 8 làm B severity=3 được chọn trước.
- Nhiều revision không tạo duplicate logical dispatch.
- Enqueue trùng ID, revision ID không tồn tại, severity ngoài [1,9] vẫn bị từ chối.
- `verify.ps1` kết thúc exit 0, toàn bộ contract PASS.

## Không reproduce được?
Kiểm tra `dotnet --info`, `pwsh --version`, chạy `dotnet run --project starter/RescueLab.csproj -- --contract` trực tiếp và kiểm tra exit code.
