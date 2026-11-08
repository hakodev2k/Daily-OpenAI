param([switch]$ExpectFixed)
$ErrorActionPreference='Stop'
$uri='http://127.0.0.1:5193/admin/permits'
$normal=Invoke-WebRequest $uri -SkipHttpErrorCheck
$spoof=Invoke-WebRequest $uri -Headers @{'X-Forwarded-For'='203.0.113.7'} -SkipHttpErrorCheck
Write-Host "Without header: $($normal.StatusCode); with claimed office IP: $($spoof.StatusCode)"
if($ExpectFixed){if($spoof.StatusCode -ne 200){throw 'Check local proxy trust policy; loopback is intentionally trusted for this harness'}; Write-Host 'Local trusted proxy simulation passed; now review untrusted peer tests in docs.'}
else {if($normal.StatusCode -ne 403 -or $spoof.StatusCode -ne 200){throw 'Expected spoof scenario not reproduced'}; Write-Host 'REPRODUCED: arbitrary header changes access decision'}
