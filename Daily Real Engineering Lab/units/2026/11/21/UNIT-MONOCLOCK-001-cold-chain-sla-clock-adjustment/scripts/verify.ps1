$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ColdChainSla.csproj'
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed.' }
