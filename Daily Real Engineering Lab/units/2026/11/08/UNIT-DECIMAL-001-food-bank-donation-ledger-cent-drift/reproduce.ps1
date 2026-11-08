$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/DonationLedger.csproj'
$output = (& dotnet run --project $project --no-launch-profile -- 0.005 0.005 0.005 | Select-Object -Last 1).Trim()
if ($LASTEXITCODE -ne 0) { throw 'dotnet run failed' }
Write-Host "Observed=$output Expected=0.03"
if ($output -eq '0.03') { throw 'Starter no longer reproduces the defect; use verify.ps1' }
Write-Host 'REPRODUCED: contract mismatch observed.'
