$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/NurseryCsv.csproj" -- test
if ($LASTEXITCODE -eq 1) { Write-Host 'REPRODUCED: starter fails the regression contract'; exit 0 }
Write-Error "Expected starter test exit 1, observed $LASTEXITCODE"; exit 1
