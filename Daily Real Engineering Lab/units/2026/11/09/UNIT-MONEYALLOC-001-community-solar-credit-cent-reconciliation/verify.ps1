$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CreditLab.csproj'
dotnet run --project $project
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: fix starter/Program.cs and retry.' }
Write-Host 'VERIFIED: learner starter satisfies settlement invariants.'
