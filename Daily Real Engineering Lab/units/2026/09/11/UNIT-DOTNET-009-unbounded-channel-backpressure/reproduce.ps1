$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/ChannelBackpressureLab.csproj"
$output | Write-Host

$produced = [int](($output | Where-Object { $_ -like 'produced=*' }) -replace 'produced=', '')
$consumed = [int](($output | Where-Object { $_ -like 'consumed=*' }) -replace 'consumed=', '')
$maxBacklog = [int](($output | Where-Object { $_ -like 'maxBacklog=*' }) -replace 'maxBacklog=', '')

if ($produced -ne 200 -or $consumed -ne 200) { throw 'Starter did not process all 200 items.' }
if ($maxBacklog -le 8) { throw "Expected original backlog symptom (> 8), observed $maxBacklog." }

Write-Host "REPRODUCED: all items complete, but maxBacklog=$maxBacklog exceeds the operating bound."
