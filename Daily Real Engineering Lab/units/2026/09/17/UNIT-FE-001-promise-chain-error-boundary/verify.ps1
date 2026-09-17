node starter/app.js
if ($LASTEXITCODE -eq 0) { Write-Host 'PASS: learner operation no longer reports success on follow-up failure.'; exit 0 }
Write-Error 'FAIL: starter still reports success before the complete operation outcome.'
exit 1
