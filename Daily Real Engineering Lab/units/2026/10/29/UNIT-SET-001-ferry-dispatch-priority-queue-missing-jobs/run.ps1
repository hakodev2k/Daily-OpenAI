$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/FerryDispatchLab.csproj') -- run
exit $LASTEXITCODE
