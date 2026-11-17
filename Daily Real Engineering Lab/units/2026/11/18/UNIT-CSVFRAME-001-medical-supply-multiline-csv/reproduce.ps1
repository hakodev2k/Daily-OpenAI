$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SupplyImport.csproj'
dotnet run --project $project -- --check
if ($LASTEXITCODE -eq 0) { throw 'Starter unexpectedly passed; original symptom not reproduced.' }
Write-Host 'REPRODUCED: starter fails CSV contract as expected.'
