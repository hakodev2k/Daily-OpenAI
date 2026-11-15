$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$output = & dotnet run --project (Join-Path $root 'starter/TransitLab.csproj') 2>&1 | Out-String
Write-Output $output
if($LASTEXITCODE -eq 0 -and $output -match 'MISMATCHES=0' -and $output -match 'Fare quote: ACTUAL=/api/v2/fares/quote'){exit 0}
Write-Error 'Learner workspace verification failed'; exit 1
