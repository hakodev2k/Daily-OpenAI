$ErrorActionPreference = 'Stop'
$output = dotnet run --project (Join-Path $PSScriptRoot 'starter/TerminationDrainLab.csproj') 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($output -match 'REQUEST R2 503' -and $output -match 'REQUEST R1 200') { Write-Host 'REPRODUCED: rollout window contains a transient request failure.'; exit 0 }
throw 'Không reproduce được symptom mong đợi.'