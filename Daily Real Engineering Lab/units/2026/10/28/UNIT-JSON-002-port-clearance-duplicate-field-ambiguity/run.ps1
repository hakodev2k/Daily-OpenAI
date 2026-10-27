$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PortManifestLab.csproj'
dotnet run --project $project -- --run
if ($LASTEXITCODE -ne 0) { throw 'Starter execution failed.' }
