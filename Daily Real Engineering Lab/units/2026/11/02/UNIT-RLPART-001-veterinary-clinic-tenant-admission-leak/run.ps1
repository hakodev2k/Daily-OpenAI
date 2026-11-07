$ErrorActionPreference = 'Stop'
& dotnet run --project (Join-Path $PSScriptRoot 'starter/TenantGateLab.csproj') -- demo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
