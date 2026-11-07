$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/TimeoutLab.csproj"
$output | ForEach-Object { Write-Host $_ }

$timeoutLine = $output | Where-Object { $_ -like 'TIMEOUTS=*' }
$activeLine = $output | Where-Object { $_ -like 'ACTIVE_AFTER_TIMEOUTS=*' }
if (-not $timeoutLine -or -not $activeLine) { throw 'Missing verification evidence.' }

$timeouts = [int](($timeoutLine -split '=')[1])
$active = [int](($activeLine -split '=')[1])

if ($timeouts -ne 6) { throw "Expected six timeout results; observed $timeouts." }
if ($active -ne 0) { throw "Verification failed: $active downstream operations still outlived the timeout boundary." }

Write-Host 'Verification passed: timeout results are preserved and losing work no longer remains active.'
