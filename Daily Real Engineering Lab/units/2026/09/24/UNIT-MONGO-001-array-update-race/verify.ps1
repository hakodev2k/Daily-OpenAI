$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/MongoRaceLab.csproj"
$output | ForEach-Object { Write-Host $_ }
$last = ($output | Select-Object -Last 1)
if ($last -notmatch 'alice' -or $last -notmatch 'bob') { throw 'Verification failed: final state must preserve both concurrent membership changes.' }
if (($last -split 'alice').Count -gt 2 -or ($last -split 'bob').Count -gt 2) { throw 'Verification failed: membership must remain idempotent.' }
Write-Host 'Verification passed: concurrent membership changes are preserved.'