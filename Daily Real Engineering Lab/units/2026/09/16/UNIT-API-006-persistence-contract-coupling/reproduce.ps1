$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ApiContractLab.csproj'
dotnet run --project $project
if ($LASTEXITCODE -eq 2) { Write-Host 'Expected contract regression reproduced.'; exit 0 }
throw 'Starter did not reproduce the expected contract regression.'