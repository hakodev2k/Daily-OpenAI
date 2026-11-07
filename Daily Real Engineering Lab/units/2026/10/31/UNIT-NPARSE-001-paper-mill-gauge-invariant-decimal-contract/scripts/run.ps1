$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root 'starter/GaugeLab.csproj') -- --report
exit $LASTEXITCODE
