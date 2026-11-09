$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/RiverGauge.csproj'
dotnet run --project $project -- --reproduce
exit $LASTEXITCODE
