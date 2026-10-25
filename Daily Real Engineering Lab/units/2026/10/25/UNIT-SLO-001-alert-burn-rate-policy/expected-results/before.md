# Before — starter
- `reproduce.ps1` exit 0: low-volume outage bị bỏ sót; high-volume within-budget bị page.
- `verify.ps1` exit khác 0; các trường hợp sai được in thành `FAIL`.
- `run.ps1` luôn in số request, số error, tỷ lệ lỗi và quyết định hiện tại.
- Nếu không reproduce: kiểm tra chưa sửa `starter/AlertPolicy.cs`, đang chạy từ unit root và .NET 8 đã cài.
