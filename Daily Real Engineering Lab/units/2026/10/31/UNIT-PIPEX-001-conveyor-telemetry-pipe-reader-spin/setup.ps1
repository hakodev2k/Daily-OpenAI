$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet restore (Join-Path $root 'starter/ConveyorTelemetry.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build (Join-Path $root 'starter/ConveyorTelemetry.csproj') --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
