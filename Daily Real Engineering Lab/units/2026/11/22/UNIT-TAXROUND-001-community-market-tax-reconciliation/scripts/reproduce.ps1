$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/MarketTaxLab.csproj'
dotnet run --project $project -- --verify
if ($LASTEXITCODE -eq 0) { Write-Error 'Expected starter failure not observed. If you already fixed it, run verify.ps1.'; exit 1 }
Write-Host 'Expected discrepancy reproduced (verification returned nonzero).'
exit 0
