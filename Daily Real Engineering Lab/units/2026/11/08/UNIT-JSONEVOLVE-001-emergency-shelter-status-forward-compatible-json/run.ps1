$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ShelterFeed.csproj'
dotnet run --project $project
exit $LASTEXITCODE
