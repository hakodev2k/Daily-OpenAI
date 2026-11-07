# Before (starter nguyên bản)

`./reproduce.ps1` exit 0 nếu fixture tái hiện đúng triệu chứng: anonymous audit 200, anonymous export 200, authenticated audit 200, anonymous health 200.

`./verify.ps1` exit 1 trước khi sửa, vì anonymous access chưa đáp ứng security contract. Nếu không reproduce được, kiểm tra SDK/runtime .NET 8, chạy trực tiếp `dotnet run --project starter/AuthBoundary.csproj -- --reproduce`, kiểm tra log status và đảm bảo chưa sửa starter.
