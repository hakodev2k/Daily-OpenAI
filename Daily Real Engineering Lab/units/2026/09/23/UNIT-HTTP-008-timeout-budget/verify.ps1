$ErrorActionPreference = 'Stop'
$failure = dotnet run --project "$PSScriptRoot/starter/TimeoutBudgetLab.csproj" -- reproduce | Out-String
Write-Host $failure
if ($failure -notmatch 'elapsedMs=([0-9]+)') { throw 'Missing failure elapsed time.' }
$failureElapsed = [int]$Matches[1]
if ($failureElapsed -gt 1200) { throw "Failure path still exceeds the request budget materially: ${failureElapsed}ms." }

$healthy = dotnet run --project "$PSScriptRoot/starter/TimeoutBudgetLab.csproj" -- healthy | Out-String
Write-Host $healthy
if ($healthy -notmatch 'RESULT success') { throw 'Healthy dependency path regressed.' }

$recover = dotnet run --project "$PSScriptRoot/starter/TimeoutBudgetLab.csproj" -- recover | Out-String
Write-Host $recover
if ($recover -notmatch 'RESULT success') { throw 'Fast recovery scenario should remain successful when budget permits.' }
Write-Host 'VERIFIED: learner-editable starter respects failure budget and preserves healthy/recovery behavior.'