$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/FerryStatus.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Run failed' }
