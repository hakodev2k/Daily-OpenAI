$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/AuditSnapshotLab.csproj') -- --verify
exit $LASTEXITCODE
