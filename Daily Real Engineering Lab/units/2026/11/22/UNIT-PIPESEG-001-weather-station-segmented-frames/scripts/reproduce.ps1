$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/TelemetryLab.csproj'
dotnet run --project $project -- reproduce
exit $LASTEXITCODE
