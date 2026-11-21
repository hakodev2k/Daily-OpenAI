$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ColdChainSla.csproj'
dotnet run --project $project -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Reproduction failed.' }
