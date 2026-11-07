$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'checks/ArchiveChecks.csproj') -- reproduce
exit $LASTEXITCODE
