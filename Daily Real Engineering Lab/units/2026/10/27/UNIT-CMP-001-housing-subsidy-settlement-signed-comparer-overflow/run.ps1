$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SettlementOrderLab.csproj'
& dotnet run --project $project -- --scenario all
exit $LASTEXITCODE
