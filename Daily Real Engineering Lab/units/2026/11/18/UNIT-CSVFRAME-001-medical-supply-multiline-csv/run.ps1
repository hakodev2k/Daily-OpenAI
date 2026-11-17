$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SupplyImport.csproj'
dotnet run --project $project --
