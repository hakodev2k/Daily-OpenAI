$ErrorActionPreference = 'Stop'
dotnet restore (Join-Path $PSScriptRoot 'starter/WaterAlerts.csproj')
exit $LASTEXITCODE
