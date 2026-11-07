$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/StreamCheckpointLab.csproj') -- verify
exit $LASTEXITCODE
