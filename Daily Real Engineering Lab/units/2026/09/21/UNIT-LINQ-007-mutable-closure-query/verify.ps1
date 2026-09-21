$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: learner-editable starter still exports a different dataset than the report-boundary expectation.' }
Write-Host 'VERIFY_PASS: report dataset remains consistent with its boundary expectation.'