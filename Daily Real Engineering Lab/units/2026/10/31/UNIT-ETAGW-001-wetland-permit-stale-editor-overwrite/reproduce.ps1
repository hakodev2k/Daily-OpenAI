$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PermitRevisionLab.csproj'
dotnet run --project $project -- reproduce
if ($LASTEXITCODE -ne 0) { throw 'Original symptom not reproduced; starter may have been fixed' }
Write-Host 'Reproduction confirmed; not a fix verification.'
