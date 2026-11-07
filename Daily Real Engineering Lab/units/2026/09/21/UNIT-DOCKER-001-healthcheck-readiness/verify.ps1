$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: the learner-editable routing health signal still accepts traffic during initialization.' }
Write-Host 'VERIFY_PASS: startup routing is gated until the application is ready.'