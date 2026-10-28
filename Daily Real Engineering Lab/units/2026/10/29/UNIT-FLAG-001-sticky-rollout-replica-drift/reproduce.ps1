$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/FlagRollout.csproj') -- reproduce
exit $LASTEXITCODE
