$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/TimeoutLab.csproj"
$output | ForEach-Object { Write-Host $_ }

$activeLine = $output | Where-Object { $_ -like 'ACTIVE_AFTER_TIMEOUTS=*' }
if (-not $activeLine) { throw 'Missing ACTIVE_AFTER_TIMEOUTS evidence.' }
$active = [int](($activeLine -split '=')[1])
if ($active -le 0) { throw 'Expected background work to remain active after timeout, but none was observed.' }

Write-Host "Reproduction succeeded: $active downstream operations were still active after callers timed out."
