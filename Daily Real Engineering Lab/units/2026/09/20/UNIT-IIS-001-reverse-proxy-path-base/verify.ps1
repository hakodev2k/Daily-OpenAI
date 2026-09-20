$ErrorActionPreference = 'Stop'
dotnet build ./starter/PathBaseLab.csproj --nologo | Out-Null
dotnet run --project ./starter/PathBaseLab.csproj --no-build -- --verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: learner-editable starter still generates an invalid public navigation URL.' }
Write-Host 'VERIFY_PASS: navigation preserves the public application boundary.'