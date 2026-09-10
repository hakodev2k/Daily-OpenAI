$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/Worker.csproj | Out-String
Write-Host $output
if ($output -notmatch 'summary: started=3 completed=3') { throw 'VERIFY FAIL: learner-editable starter did not complete all accepted jobs before shutdown.' }
if ($output -notmatch 'worker: ExecuteAsync returning') { throw 'VERIFY FAIL: expected worker lifecycle evidence was missing.' }
Write-Host 'VERIFY PASS: all accepted jobs completed before worker lifecycle ended.'
