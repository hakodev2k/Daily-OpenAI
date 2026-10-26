# Before
`./reproduce.ps1` exit 0 khi starter còn mismatch; `./run.ps1` exit khác 0. Dự kiến 4 FAIL: valid v1, missing v2 hazard, unknown version, mixed fields. Nếu build lỗi, kiểm tra `dotnet --list-sdks` và .NET 10 SDK.
