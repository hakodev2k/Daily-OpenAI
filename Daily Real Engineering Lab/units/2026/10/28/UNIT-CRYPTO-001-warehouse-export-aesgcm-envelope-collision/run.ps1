$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/EnvelopeIncident.csproj'
dotnet run --project $project -- --demo
if ($LASTEXITCODE -ne 0) { throw 'Demo failed.' }
