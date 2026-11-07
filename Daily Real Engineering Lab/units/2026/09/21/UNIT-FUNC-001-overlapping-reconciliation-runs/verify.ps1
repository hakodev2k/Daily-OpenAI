$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: a schedule window was duplicated or a later window was suppressed.' }
Write-Host 'VERIFY_PASS: each logical window executes once across competing hosts.'