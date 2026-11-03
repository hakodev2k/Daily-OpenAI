$ErrorActionPreference='Stop'
dotnet restore (Join-Path $PSScriptRoot 'starter/HarborArrival.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
