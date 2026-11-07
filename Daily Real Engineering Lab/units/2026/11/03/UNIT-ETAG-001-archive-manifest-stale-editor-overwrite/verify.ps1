$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/ArchiveManifest.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
