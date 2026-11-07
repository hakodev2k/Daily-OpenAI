$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet restore (Join-Path $root 'contracts/ReplayContracts.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build (Join-Path $root 'contracts/ReplayContracts.csproj') --no-restore
exit $LASTEXITCODE
