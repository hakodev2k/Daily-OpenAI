$output = & "$PSScriptRoot/run.ps1" 2>&1 | Out-String
$output
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: secure request still cannot proceed.' }
if ($output -notmatch 'RESULT=HTTPS_REQUEST_CAN_PROCEED') { throw 'VERIFY_FAIL: expected success evidence is missing.' }
Write-Host 'VERIFY_PASS'