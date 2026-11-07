$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: learner-editable pipeline still violates required lifecycle or payload behavior.' }
Write-Host 'VERIFY_PASS: downstream receives the complete payload.'