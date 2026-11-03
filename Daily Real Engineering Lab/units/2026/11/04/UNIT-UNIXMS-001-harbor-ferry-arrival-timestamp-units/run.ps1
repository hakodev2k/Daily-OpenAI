$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/HarborArrival.csproj')
exit $LASTEXITCODE
