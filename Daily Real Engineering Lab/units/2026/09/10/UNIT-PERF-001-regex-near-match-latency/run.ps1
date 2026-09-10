$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RegexIncident.csproj'
dotnet run --project $project -- --run
