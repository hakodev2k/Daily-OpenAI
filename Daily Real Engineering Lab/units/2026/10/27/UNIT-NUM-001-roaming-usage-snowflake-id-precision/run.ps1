$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RoamingUsage.csproj'
& dotnet run --project $project -- --demo
if ($LASTEXITCODE -ne 0) { throw "Starter run failed with exit code $LASTEXITCODE" }
