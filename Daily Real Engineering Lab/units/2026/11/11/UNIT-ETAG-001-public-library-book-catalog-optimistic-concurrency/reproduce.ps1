$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/LibraryCatalog.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { throw 'Reproduction failed' }
