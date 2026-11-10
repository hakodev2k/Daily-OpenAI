$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/WeatherLab.csproj') -- --verify
exit $LASTEXITCODE
