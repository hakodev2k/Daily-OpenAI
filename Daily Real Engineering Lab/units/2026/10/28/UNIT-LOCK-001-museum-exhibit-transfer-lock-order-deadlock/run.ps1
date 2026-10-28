$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ExhibitLedger.csproj'
& dotnet run --project $project -c Release
exit $LASTEXITCODE
