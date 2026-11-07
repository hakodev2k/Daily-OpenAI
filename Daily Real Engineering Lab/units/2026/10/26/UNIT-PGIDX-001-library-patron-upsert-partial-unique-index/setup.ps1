$ErrorActionPreference = "Stop"
docker compose -f "$PSScriptRoot/compose.yaml" up -d --wait
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
docker compose -f "$PSScriptRoot/compose.yaml" exec -T postgres psql -X -v ON_ERROR_STOP=1 -U lab -d lab -f /lab/schema/001_schema.sql
exit $LASTEXITCODE
