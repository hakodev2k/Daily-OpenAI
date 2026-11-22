$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $unit 'starter/ScanAudit.csproj') -- verify
exit $LASTEXITCODE
