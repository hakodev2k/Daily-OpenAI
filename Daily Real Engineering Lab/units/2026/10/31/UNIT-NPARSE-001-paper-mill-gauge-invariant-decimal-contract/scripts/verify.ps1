$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root 'starter/GaugeLab.csproj') -- --expect-fixed
exit $LASTEXITCODE
