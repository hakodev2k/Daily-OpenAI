$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/BridgeBriefingLab.csproj') 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
$report = $output -join "`n"
if ($code -ne 0 -or $report -notmatch 'RESULT 4/4' -or $report -match 'FAIL ') {
    throw "Verification failed: learner-editable starter must pass all 4 fixtures, including cache-hit and missing-origin behavior. Exit $code."
}
Write-Host 'Verified: learner starter passes 4/4 functional and cache-property fixtures.'
