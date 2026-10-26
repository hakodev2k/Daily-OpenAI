$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ClaimLogLab.csproj'
dotnet run --project $project -- --reproduce
if ($LASTEXITCODE -ne 0) { throw '--reproduce failed; inspect console output.' }
