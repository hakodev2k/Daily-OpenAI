$ErrorActionPreference='Stop'
npx playwright test
if($LASTEXITCODE -ne 0){ throw 'Verification failed. The learner-editable test is still timing-dependent or incorrect.' }
Write-Host 'Verification passed.'
