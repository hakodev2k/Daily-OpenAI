$ErrorActionPreference = 'Stop'
$compose = Join-Path $PSScriptRoot 'compose.yaml'
& docker compose -f $compose up -d --wait mongo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& docker compose -f $compose exec -T mongo mongosh --quiet --norc --file /lab/starter/fixture.js
exit $LASTEXITCODE
