$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/FlagRollout.csproj') -- verify
exit $LASTEXITCODE
