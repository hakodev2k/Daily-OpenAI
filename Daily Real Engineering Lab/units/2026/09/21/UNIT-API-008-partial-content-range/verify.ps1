$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: learner-editable starter still violates one or more response invariants.' }
Write-Host 'VERIFY_PASS'