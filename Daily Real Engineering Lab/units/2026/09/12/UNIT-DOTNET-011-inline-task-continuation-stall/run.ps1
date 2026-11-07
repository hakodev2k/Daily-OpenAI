$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/InlineContinuationLab.csproj'
dotnet run --project $project
