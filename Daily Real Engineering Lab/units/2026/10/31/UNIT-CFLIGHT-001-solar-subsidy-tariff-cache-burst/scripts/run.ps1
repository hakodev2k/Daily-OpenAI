$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root 'starter/Starter.csproj') -- --report
exit $LASTEXITCODE
