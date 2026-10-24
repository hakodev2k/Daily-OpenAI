$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/AuditLab.Starter.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
