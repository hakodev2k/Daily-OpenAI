$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/../starter/AquariumGate.csproj" --no-restore
if ($LASTEXITCODE -ne 0) { throw "FAIL: learner workspace exited $LASTEXITCODE" }
Write-Host 'PASS: learner workspace preserves exclusive gate'
