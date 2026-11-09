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
# psql continues after an SQL error to expose the following transaction symptoms.
Get-Content -Raw starter/import.sql | docker compose exec -T postgres psql -X -v ON_ERROR_STOP=0 -v VERBOSITY=verbose -U lab -d lab 2>&1 | Out-Host
Get-Content -Raw starter/inspect.sql | docker compose exec -T postgres psql -X -v ON_ERROR_STOP=1 -U lab -d lab | Out-Host
Write-Host 'Record the SQLSTATE and row totals; reproduce is not a success assertion.'
