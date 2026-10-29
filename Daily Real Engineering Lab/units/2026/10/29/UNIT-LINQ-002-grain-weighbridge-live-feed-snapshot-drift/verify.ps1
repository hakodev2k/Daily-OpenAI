$ErrorActionPreference = 'Stop'
$unit = $PSScriptRoot
dotnet run --project (Join-Path $unit 'starter/WeighbridgeLab.csproj') -- verify
exit $LASTEXITCODE
