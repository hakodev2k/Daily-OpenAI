# Before — starter

- `/health`: 200 `healthy`, không audit.
- Hai route `/api/v1/maintenance/*`: 404, body rỗng, audit=`audit-v1`.
- `/missing`: 404, body rỗng, không audit.
- `reproduce.ps1`: exit 0; `verify.ps1`: exit 1.

Nếu kết quả khác, chạy `dotnet run --project ./starter/PipelineLab.csproj -- inspect` và so sánh output. Reproduce kiểm tra symptom, không phải fix.
