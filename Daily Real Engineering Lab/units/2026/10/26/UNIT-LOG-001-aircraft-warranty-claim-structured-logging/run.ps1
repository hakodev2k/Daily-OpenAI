$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ClaimLogLab.csproj'
dotnet run --project $project -- --run
if ($LASTEXITCODE -ne 0) { throw '--run failed; inspect console output.' }
