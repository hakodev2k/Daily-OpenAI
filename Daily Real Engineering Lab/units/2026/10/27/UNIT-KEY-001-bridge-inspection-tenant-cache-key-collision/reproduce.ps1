$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/BridgeBriefingLab.csproj') 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
$report = $output -join "`n"
if ($code -ne 12 -or $report -notmatch 'PASS NORMAL_REPEAT' -or
    $report -notmatch 'FAIL COLLISION_FORWARD' -or
    $report -notmatch 'FAIL COLLISION_REVERSE' -or
    $report -notmatch 'FAIL MISSING_ORIGIN' -or
    $report -notmatch 'RESULT 1/4') {
    throw "Reproduction failed: expected original 1/4 signature and exit 12; got $code. Build errors are not valid reproduction."
}
Write-Host 'Reproduced: synthetic cross-tenant briefing symptom (1/4 baseline).'
