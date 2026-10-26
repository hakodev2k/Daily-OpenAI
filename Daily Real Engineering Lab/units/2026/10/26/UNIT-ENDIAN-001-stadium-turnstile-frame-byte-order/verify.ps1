$ErrorActionPreference = 'Stop'
& dotnet run --project (Join-Path $PSScriptRoot 'starter/GateFrameLab.csproj') -- --verify
exit $LASTEXITCODE
