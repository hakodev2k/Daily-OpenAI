# Expected results

| Phase | Restarts | Rejected | Accepted | Lost in-flight |
|---|---:|---:|---:|---:|
| Starter | 2 | 2 | 2 | 3 |
| Fixed | 0 | 2 | 2 | 0 |

Không dùng timing để pass/fail. Nếu không reproduce: xác nhận SDK 8, chạy tại unit root, xem output của `dotnet run --project starter/ProbeLab.csproj`. Không sử dụng cloud credentials.
