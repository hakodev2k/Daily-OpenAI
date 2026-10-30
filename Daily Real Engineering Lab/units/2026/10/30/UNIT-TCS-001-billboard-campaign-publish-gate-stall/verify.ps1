$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SignalGateLab.csproj'
dotnet run --project $project --configuration Release -- --verify
exit $LASTEXITCODE
