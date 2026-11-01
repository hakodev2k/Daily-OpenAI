$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $root 'starter/ReplayLab.csproj')
exit $LASTEXITCODE
