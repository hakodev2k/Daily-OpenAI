# Kiểm chứng trực tiếp mã learner đã sửa trong starter/, không chạy reference solution.
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/LeaseLab.csproj'
dotnet run --project $project --configuration Release -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification gate failed: crash recovery or normal reservation invariant is violated.' }
