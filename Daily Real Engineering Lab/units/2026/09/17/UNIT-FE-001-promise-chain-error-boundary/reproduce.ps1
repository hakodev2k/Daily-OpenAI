node starter/app.js
if ($LASTEXITCODE -eq 2) { Write-Host 'Reproduced: operation reported success although follow-up failed.'; exit 0 }
Write-Error 'Expected starter symptom was not reproduced.'
exit 1
