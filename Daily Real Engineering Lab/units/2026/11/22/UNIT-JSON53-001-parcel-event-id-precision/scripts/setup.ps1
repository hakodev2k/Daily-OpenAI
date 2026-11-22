$ErrorActionPreference = 'Stop'
dotnet --version
dotnet restore (Join-Path $PSScriptRoot '../starter/ParcelEvents.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
