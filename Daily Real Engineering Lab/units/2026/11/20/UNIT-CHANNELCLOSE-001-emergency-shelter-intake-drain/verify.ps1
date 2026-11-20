$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/ShelterIntake.csproj') -- --check 2>&1 | Out-String
$exit = $LASTEXITCODE
Write-Output $output
if ($exit -ne 0 -or $output -notmatch 'accepted=2; processed=2; missing=' -or $output -match 'write rejected') { throw 'Learner starter failed verification.' }
Write-Output 'VERIFIED: both accepted records processed.'
