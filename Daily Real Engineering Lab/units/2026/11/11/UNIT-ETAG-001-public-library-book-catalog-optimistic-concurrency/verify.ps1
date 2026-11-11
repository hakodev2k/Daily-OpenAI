$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/LibraryCatalog.csproj') -- verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed' }
