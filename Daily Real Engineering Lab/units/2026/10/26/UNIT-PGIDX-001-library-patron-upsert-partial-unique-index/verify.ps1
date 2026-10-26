$ErrorActionPreference = "Stop"
& "$PSScriptRoot/setup.ps1"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
docker compose -f "$PSScriptRoot/compose.yaml" exec -T postgres psql -X -v ON_ERROR_STOP=1 -U lab -d lab -f /lab/harness/verify.sql
exit $LASTEXITCODE
