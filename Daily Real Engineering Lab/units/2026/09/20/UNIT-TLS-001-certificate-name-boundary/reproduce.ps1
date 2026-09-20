$output = & "$PSScriptRoot/run.ps1" 2>&1 | Out-String
$output
if ($output -notmatch 'TCP reachable: True') { throw 'Expected reachable network path was not reproduced.' }
if ($output -notmatch 'RESULT=IDENTITY_VALIDATION_FAILURE') { throw 'Expected secure-connection symptom was not reproduced.' }
Write-Host 'REPRODUCE_PASS'