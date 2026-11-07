$ErrorActionPreference = 'Stop'
$unit = $PSScriptRoot
dotnet run --project (Join-Path $unit 'starter/WeighbridgeLab.csproj') -- reproduce
exit $LASTEXITCODE
