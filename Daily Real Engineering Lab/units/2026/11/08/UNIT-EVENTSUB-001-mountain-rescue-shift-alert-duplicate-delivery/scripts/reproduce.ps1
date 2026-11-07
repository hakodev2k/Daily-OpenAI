$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/RescueAlerts.csproj'
dotnet run --project $project -- --reproduce
exit $LASTEXITCODE
