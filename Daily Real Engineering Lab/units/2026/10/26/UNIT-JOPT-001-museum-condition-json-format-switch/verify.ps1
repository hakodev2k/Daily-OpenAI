$ErrorActionPreference = 'Stop'
& dotnet run --project (Join-Path $PSScriptRoot 'starter/ConditionLab.csproj') -- --verify
exit $LASTEXITCODE
