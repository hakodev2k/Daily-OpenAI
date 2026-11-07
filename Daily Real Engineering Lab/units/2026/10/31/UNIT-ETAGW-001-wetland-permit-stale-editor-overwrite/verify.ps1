$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PermitRevisionLab.csproj'
dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) { throw 'Learner starter failed the conditional-write contract' }
Write-Host 'Learner starter passed verification.'
