$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Starter.csproj'
dotnet run --project $project --no-launch-profile --urls http://127.0.0.1:5187
