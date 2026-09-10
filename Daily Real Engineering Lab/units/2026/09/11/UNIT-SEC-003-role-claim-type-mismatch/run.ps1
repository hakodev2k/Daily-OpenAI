$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RoleClaimLab.csproj'
dotnet run --project $project
