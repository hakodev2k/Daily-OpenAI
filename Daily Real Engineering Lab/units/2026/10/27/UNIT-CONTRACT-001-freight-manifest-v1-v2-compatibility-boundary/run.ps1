$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/FreightContracts.csproj'
dotnet run --project $project -- --self-test
exit $LASTEXITCODE
