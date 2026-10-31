$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project (Join-Path $root 'starter/ConveyorTelemetry.csproj') -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
