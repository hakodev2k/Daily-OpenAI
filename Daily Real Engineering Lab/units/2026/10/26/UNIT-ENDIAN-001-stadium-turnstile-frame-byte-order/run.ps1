$ErrorActionPreference = 'Stop'
& dotnet run --project (Join-Path $PSScriptRoot 'starter/GateFrameLab.csproj') -- --inspect
exit $LASTEXITCODE
