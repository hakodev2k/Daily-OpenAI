$ErrorActionPreference = 'Stop'
& dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& dotnet restore (Join-Path $PSScriptRoot 'starter/TenantGateLab.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
