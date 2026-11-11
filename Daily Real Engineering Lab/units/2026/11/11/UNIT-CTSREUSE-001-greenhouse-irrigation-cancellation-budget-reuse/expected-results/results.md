# Expected Results

| Phase | Cycle 1 | Cycle 2 |
|---|---|---|
| Original starter | status=ok | status=cancelled |
| Learner fixed starter | status=ok | status=ok |

Không phụ thuộc chính xác elapsedMs. `reproduce.ps1` kiểm tra symptom gốc, còn `verify.ps1` kiểm tra chính code learner sửa trong `starter/`. Không có external dependency.

Nếu không reproduce được, kiểm tra SDK `dotnet --version`, dùng starter nguyên bản và chạy `dotnet run --project ./starter/Greenhouse.csproj`.
