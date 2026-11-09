$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ConservationLab.csproj'
$output = & dotnet run --project $project -- verify 2>&1
$output | ForEach-Object { Write-Output $_ }
if ($LASTEXITCODE -ne 0 -or -not (($output -join ' ') -match 'PASS: priority and note preserved')) { throw 'Verification failed against learner-edited starter.' }
