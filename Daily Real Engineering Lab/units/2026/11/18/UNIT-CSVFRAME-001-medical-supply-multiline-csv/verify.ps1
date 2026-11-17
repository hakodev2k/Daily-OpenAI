$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SupplyImport.csproj'
dotnet run --project $project -- --check
if ($LASTEXITCODE -ne 0) { throw 'VERIFY FAILED: learner starter does not satisfy contract.' }
Write-Host 'VERIFY PASSED: learner starter satisfies CSV contract.'
