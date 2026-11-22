$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore (Join-Path $unit 'starter/ScanAudit.csproj')
exit $LASTEXITCODE
