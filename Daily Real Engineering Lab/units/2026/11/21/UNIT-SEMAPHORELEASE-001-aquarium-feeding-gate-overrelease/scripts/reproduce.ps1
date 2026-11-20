$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/../starter/AquariumGate.csproj" --no-restore
if ($LASTEXITCODE -eq 2) { Write-Host 'PASS: expected starter failure reproduced'; exit 0 }
throw "Expected starter failure exit 2, got $LASTEXITCODE"
