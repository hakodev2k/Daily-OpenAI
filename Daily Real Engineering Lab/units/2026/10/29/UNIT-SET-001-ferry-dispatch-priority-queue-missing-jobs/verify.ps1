$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/FerryDispatchLab.csproj') -- verify
exit $LASTEXITCODE
