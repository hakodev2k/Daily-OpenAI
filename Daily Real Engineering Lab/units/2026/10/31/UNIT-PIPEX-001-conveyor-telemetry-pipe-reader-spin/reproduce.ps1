$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project (Join-Path $root 'starter/ConveyorTelemetry.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
