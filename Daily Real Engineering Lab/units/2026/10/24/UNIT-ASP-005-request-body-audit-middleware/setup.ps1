$ErrorActionPreference = 'Stop'
dotnet restore (Join-Path $PSScriptRoot 'starter/AuditLab.Starter.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore (Join-Path $PSScriptRoot 'solution/AuditLab.Solution.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
