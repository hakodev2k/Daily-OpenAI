$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/HarborArrival.csproj') -- --verify
exit $LASTEXITCODE
