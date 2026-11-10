$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = dotnet run --project (Join-Path $root 'starter/HarborLab.csproj')
$output
if ($LASTEXITCODE -ne 0) { throw 'Starter failed to execute' }
if (-not ($output -match 'mode after old ack: MAINTENANCE')) { throw 'Expected symptom was not reproduced' }
Write-Host 'REPRODUCED: stale acknowledgment overwrote latest mode'
