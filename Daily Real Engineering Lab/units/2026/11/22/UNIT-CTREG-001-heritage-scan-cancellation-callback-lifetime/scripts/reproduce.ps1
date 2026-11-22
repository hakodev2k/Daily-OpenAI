$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $unit 'starter/ScanAudit.csproj') -- reproduce
exit $LASTEXITCODE
