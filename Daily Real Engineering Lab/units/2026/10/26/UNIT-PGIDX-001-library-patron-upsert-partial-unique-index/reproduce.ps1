$ErrorActionPreference = "Stop"
& "$PSScriptRoot/setup.ps1"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
$output = docker compose -f "$PSScriptRoot/compose.yaml" exec -T postgres psql -X -v ON_ERROR_STOP=1 -U lab -d lab -f /lab/harness/reproduce.sql 2>&1 | Out-String
$code = $LASTEXITCODE
Write-Output $output
if ($code -ne 0 -and $output.Contains("no unique or exclusion constraint matching the ON CONFLICT specification")) {
    Write-Output "REPRODUCE PASS"
    exit 0
}
Write-Error "REPRODUCE FAIL: expected database diagnostic not observed"
exit 1
