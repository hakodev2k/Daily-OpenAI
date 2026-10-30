$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'contracts/ProbeContract.csproj'
dotnet run --project $project -c Release -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification FAILED: learner starter does not meet the probe contract.' }
Write-Host 'Verification passed on learner starter.'
