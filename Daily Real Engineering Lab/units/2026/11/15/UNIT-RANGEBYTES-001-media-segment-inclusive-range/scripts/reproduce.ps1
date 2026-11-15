$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $root 'starter/RangeLab.csproj') -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
