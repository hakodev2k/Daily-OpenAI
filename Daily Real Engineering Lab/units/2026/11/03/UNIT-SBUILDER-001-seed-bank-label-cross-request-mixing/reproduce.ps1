$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SeedBank.Labels.csproj'
dotnet run --project $project --configuration Release -- --reproduce
exit $LASTEXITCODE
