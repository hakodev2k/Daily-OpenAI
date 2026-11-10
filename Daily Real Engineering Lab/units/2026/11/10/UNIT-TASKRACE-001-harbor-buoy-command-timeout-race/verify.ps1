$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = dotnet run --project (Join-Path $root 'solution/HarborLab.csproj')
$output
if ($LASTEXITCODE -ne 0) { throw 'Reference failed to execute' }
if (-not ($output -match 'mode after old ack: ACTIVE')) { throw 'Reference invariant failed' }
Write-Host 'PASS: late ACK cannot overwrite latest mode'
