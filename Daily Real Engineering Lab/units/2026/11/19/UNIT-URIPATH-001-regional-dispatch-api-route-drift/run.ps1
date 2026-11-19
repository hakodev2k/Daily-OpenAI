$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/RegionalDispatch.csproj') -- run
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
