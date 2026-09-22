$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/TimeoutBudgetLab.csproj" -- reproduce | Out-String
Write-Host $output
if ($output -notmatch 'attempts=3') { throw 'Expected three downstream attempts in starter failure scenario.' }
if ($output -notmatch 'elapsedMs=([0-9]+)') { throw 'Missing elapsed time.' }
$elapsed = [int]$Matches[1]
if ($elapsed -lt 1800) { throw "Starter symptom not reproduced: elapsed ${elapsed}ms." }
Write-Host "REPRODUCED: request work exceeded the 900ms caller deadline; elapsed=${elapsed}ms."