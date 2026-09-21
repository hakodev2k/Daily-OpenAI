$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: learner-editable starter is still order-dependent.' }
Write-Host 'VERIFY_PASS: checks remain correct after the preceding locale-specific scenario.'