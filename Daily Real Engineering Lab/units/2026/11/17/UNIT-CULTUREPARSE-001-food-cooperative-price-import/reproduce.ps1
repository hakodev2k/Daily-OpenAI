$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PriceImport.csproj'
dotnet run --project $project -- --demo
if ($LASTEXITCODE -ne 0) { throw 'Starter reproduction command failed to run' }
Write-Host 'Record the observed locale differences in workspace/my-investigation.md.'
