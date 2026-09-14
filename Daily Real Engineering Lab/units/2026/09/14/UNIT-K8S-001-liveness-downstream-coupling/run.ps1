$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
dotnet run --project $project --urls http://127.0.0.1:5199
