$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/GridSignalLab.csproj'
dotnet run --project $project -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Baseline run failed.' }
