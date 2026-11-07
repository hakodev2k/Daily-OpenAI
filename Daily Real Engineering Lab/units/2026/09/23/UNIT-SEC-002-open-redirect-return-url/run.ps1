$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PortalLab.csproj'
dotnet restore $project
dotnet run --project $project