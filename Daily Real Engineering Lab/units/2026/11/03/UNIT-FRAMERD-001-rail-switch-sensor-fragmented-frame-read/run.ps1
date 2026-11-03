$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/FrameLab.csproj') -- demo
exit $LASTEXITCODE
