$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'contracts/ProbeContract.csproj'
dotnet run --project $project -c Release -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Reproduce FAILED: starter did not exhibit the documented symptom.' }
Write-Host 'Reproduction confirmed.'
