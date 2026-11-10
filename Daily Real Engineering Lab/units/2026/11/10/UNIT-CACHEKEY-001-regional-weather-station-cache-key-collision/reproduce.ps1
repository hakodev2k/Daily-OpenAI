$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/WeatherLab.csproj') -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Reproduction did not show expected initial symptom.' }
