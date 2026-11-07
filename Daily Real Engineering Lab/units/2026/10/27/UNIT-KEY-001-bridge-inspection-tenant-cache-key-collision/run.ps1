$ErrorActionPreference = 'Stop'
& dotnet run --project (Join-Path $PSScriptRoot 'starter/BridgeBriefingLab.csproj')
exit $LASTEXITCODE
