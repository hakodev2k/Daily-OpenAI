$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/ParcelEvents.csproj')
exit $LASTEXITCODE
