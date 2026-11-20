$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/ShelterIntake.csproj') -- --check 2>&1 | Out-String
$exit = $LASTEXITCODE
Write-Output $output
if ($exit -eq 0 -or $output -notmatch 'INVARIANT FAILED' -or $output -notmatch 'missing=south-205') { throw 'Original symptom not reproduced; restore starter.' }
Write-Output 'REPRODUCED: original starter violates the invariant.'
