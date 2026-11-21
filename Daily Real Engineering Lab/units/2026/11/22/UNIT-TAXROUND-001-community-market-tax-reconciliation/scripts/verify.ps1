$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/MarketTaxLab.csproj'
dotnet run --project $project -- --verify
exit $LASTEXITCODE
