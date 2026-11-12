$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet --version
if ($LASTEXITCODE -ne 0) { exit 1 }
dotnet restore (Join-Path $root 'starter/ParcelLab.csproj')
exit $LASTEXITCODE
