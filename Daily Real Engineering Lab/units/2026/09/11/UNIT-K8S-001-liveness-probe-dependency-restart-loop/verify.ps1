$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/HealthProbeSimulator.csproj
$output | Write-Host
$result = $output | Select-String 'RESULT restartCount=' | Select-Object -Last 1
if (-not $result) { throw 'Missing RESULT line.' }
$count = [int](($result.ToString() -split '=')[-1])
if ($count -ne 0) { throw "Expected restartCount=0 after the learner fix, got $count." }
$downLines = $output | Select-String 'db=DOWN'
if ($downLines.Count -eq 0) { throw 'Dependency outage was not exercised.' }
if (($downLines | Where-Object { $_.ToString() -notmatch 'ready=False' }).Count -gt 0) { throw 'Readiness must become false while the database is unavailable.' }
Write-Host 'Verified: no restart during transient dependency outage, while readiness still rejects traffic.'
