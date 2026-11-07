$ErrorActionPreference='Stop'
npx playwright test
$exit=$LASTEXITCODE
if($exit -eq 0){ throw 'Expected original starter test to fail, but it passed.' }
Write-Host 'Reproduction confirmed: starter E2E test fails before the UI reaches its final state.'
exit 0
