$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/RetryRequestMessageReuse.csproj 2>&1 | Out-String
Write-Host $output
if ($output -notmatch 'RESULT=OK' -or $output -notmatch 'STATUS=200' -or $output -notmatch 'ATTEMPTS=2') {
    throw 'Verification failed: learner-editable starter does not complete the retry successfully.'
}
Write-Host 'VERIFY=PASS'
