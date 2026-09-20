$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --sequential
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: public sequential behavior regressed.' }
dotnet run --project ./starter/Starter.csproj -- --verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: parallel statements are not isolated.' }
Write-Host 'VERIFY_PASS: output contract is preserved and coordinated parallel rendering is isolated.'