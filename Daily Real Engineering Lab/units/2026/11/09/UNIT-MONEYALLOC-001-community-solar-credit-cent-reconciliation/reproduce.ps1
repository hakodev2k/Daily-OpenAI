$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CreditLab.csproj'
dotnet run --project $project
if ($LASTEXITCODE -eq 0) { throw 'Unexpected pass: starter must reproduce settlement mismatch.' }
Write-Host 'REPRODUCED: starter violates the settlement contract.'
