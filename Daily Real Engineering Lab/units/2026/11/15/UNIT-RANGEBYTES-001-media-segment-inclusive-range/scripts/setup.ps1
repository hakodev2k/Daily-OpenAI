$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet --version
dotnet build (Join-Path $root 'starter/RangeLab.csproj') -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
