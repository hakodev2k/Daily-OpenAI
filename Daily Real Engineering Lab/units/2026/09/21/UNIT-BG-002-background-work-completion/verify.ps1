$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: batch completion/failure contract is still incorrect.' }
Write-Host 'VERIFY_PASS: required work is awaited and item failure is observed by the batch.'