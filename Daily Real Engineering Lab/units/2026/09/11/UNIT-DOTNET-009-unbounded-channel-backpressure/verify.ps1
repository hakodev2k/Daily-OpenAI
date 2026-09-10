$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/ChannelBackpressureLab.csproj"
$output | Write-Host

$produced = [int](($output | Where-Object { $_ -like 'produced=*' }) -replace 'produced=', '')
$consumed = [int](($output | Where-Object { $_ -like 'consumed=*' }) -replace 'consumed=', '')
$maxBacklog = [int](($output | Where-Object { $_ -like 'maxBacklog=*' }) -replace 'maxBacklog=', '')

if ($produced -ne 200) { throw "Expected produced=200, observed $produced." }
if ($consumed -ne 200) { throw "Expected consumed=200, observed $consumed. Work must not be dropped." }
if ($maxBacklog -gt 8) { throw "Expected maxBacklog <= 8 after learner fix, observed $maxBacklog." }

Write-Host "VERIFIED: all 200 items completed and maxBacklog=$maxBacklog is bounded."
