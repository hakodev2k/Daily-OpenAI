$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ShelterQuotaLab.csproj'
dotnet run --project $project --configuration Release
exit $LASTEXITCODE
