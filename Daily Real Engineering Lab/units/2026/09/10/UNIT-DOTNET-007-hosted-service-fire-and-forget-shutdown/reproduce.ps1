$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/Worker.csproj | Out-String
Write-Host $output
if ($output -notmatch 'summary: started=3 completed=0') { throw 'Starter did not reproduce the expected work-loss symptom.' }
Write-Host 'REPRODUCE PASS: accepted jobs were not completed before worker lifecycle ended.'
