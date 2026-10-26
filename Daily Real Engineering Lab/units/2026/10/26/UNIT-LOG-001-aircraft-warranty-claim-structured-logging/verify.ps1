$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ClaimLogLab.csproj'
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { throw '--verify failed; inspect console output.' }
