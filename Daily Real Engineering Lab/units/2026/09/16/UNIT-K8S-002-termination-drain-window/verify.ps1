$ErrorActionPreference = 'Stop'
$output = dotnet run --project (Join-Path $PSScriptRoot 'starter/TerminationDrainLab.csproj') 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($output -match 'failed=0' -and $output -match 'REQUEST R1 200' -and $output -match 'REQUEST R2 REJECTED') { Write-Host 'VERIFIED: new traffic is removed from the terminating instance before unsafe shutdown.'; exit 0 }
throw 'Lifecycle/traffic contract chưa đạt yêu cầu.'