$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..'))
& docker compose -p unit-sql-009 up -d --wait
if ($LASTEXITCODE -ne 0) { throw 'Docker startup failed' }
& docker compose -p unit-sql-009 exec -T db psql -X -U lab -d lab -v ON_ERROR_STOP=1 -f /lab/scripts/reset.sql
if ($LASTEXITCODE -ne 0) { throw 'SQL reset failed' }
