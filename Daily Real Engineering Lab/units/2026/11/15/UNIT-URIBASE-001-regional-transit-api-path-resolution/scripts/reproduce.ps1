$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$output = & dotnet run --project (Join-Path $root 'starter/TransitLab.csproj') 2>&1 | Out-String
Write-Output $output
if($output -match 'MISMATCHES=2' -and $output -match 'Fare quote: ACTUAL=/fares/quote'){exit 0}
Write-Error 'Original symptom not reproduced'; exit 1
