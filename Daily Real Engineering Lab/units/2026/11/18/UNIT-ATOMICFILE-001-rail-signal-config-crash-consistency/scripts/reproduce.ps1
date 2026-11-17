$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $unit 'starter/SignalConfigLab.csproj') -- reproduce
exit $LASTEXITCODE
