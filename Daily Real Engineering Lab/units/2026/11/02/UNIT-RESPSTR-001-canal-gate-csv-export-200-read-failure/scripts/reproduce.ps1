$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $unit 'harness/Probe.csproj') -- reproduce
exit $LASTEXITCODE
