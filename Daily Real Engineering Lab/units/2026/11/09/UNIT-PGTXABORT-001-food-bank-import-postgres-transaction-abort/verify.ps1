$ErrorActionPreference = 'Stop'
docker compose up -d | Out-Host
$ready = $false
for ($i=0; $i -lt 30; $i++) {
  docker compose exec -T postgres pg_isready -U lab -d lab *> $null
  if ($LASTEXITCODE -eq 0) { $ready = $true; break }
  Start-Sleep -Seconds 1
}
if (-not $ready) { throw 'PostgreSQL not ready' }
Get-Content -Raw starter/fixture.sql | docker compose exec -T postgres psql -X -v ON_ERROR_STOP=1 -U lab -d lab | Out-Host
Get-Content -Raw starter/import.sql | docker compose exec -T postgres psql -X -v ON_ERROR_STOP=1 -U lab -d lab | Out-Host
if ($LASTEXITCODE -ne 0) { throw 'Import failed' }
$actual = (docker compose exec -T postgres psql -X -At -U lab -d lab -c "SELECT string_agg(donation_id || ':' || amount_cents, ',' ORDER BY donation_id) FROM donations;").Trim()
if ($LASTEXITCODE -ne 0 -or $actual -ne 'D-100:1200,D-101:2500,D-102:3100') { throw "Incorrect reconciliation: $actual" }
Write-Host 'PASS: original + two valid donations; duplicate did not alter total.'
