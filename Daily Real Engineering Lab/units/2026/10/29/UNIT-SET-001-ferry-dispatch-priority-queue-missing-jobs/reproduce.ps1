$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/FerryDispatchLab.csproj') -- reproduce
exit $LASTEXITCODE
