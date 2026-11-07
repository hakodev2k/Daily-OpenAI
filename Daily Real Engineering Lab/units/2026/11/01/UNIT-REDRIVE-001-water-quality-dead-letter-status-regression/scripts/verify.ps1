$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $root 'contracts/ReplayContracts.csproj') -- --verify
exit $LASTEXITCODE
