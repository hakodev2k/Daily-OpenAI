$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/RetryRequestMessageReuse.csproj 2>&1 | Out-String
Write-Host $output
if ($output -notmatch 'RESULT=FAILED' -or $output -notmatch 'EXCEPTION=InvalidOperationException' -or $output -notmatch 'ATTEMPTS=1') {
    throw 'Starter did not reproduce the expected retry failure.'
}
Write-Host 'REPRODUCE=PASS'
