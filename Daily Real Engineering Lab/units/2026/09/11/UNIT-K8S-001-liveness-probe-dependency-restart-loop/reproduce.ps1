$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/HealthProbeSimulator.csproj
$output | Write-Host
$result = $output | Select-String 'RESULT restartCount=' | Select-Object -Last 1
if (-not $result) { throw 'Missing RESULT line.' }
$count = [int](($result.ToString() -split '=')[-1])
if ($count -le 0) { throw 'Expected the starter to demonstrate at least one simulated restart.' }
Write-Host "Reproduced: transient dependency outage caused $count simulated restarts."
