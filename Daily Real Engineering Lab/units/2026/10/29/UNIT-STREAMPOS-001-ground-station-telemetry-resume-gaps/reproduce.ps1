$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/StreamCheckpointLab.csproj') -- reproduce
exit $LASTEXITCODE
