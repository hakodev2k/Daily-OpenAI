$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/PermitFeed.csproj') --
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
