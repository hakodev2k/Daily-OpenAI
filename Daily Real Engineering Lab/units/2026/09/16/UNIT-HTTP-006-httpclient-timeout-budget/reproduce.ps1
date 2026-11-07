$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot 'starter/DeadlineLab.csproj'
$output = dotnet run --project $project 2>&1 | Out-String
Write-Host $output
if ($output -notmatch 'RESULT=INVENTORY\+CARRIER') { throw 'Starter did not complete both downstream calls as expected.' }
if ($output -notmatch 'ELAPSED_MS=(\d+)') { throw 'Elapsed measurement missing.' }
$elapsed=[int]$Matches[1]
if ($elapsed -lt 1100) { throw "Expected cumulative latency above business budget; observed ${elapsed}ms." }
Write-Host 'REPRODUCED: operation exceeded the 900ms business budget.'