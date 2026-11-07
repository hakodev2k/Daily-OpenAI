$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/StreamCheckpointLab.csproj') -- run
exit $LASTEXITCODE
