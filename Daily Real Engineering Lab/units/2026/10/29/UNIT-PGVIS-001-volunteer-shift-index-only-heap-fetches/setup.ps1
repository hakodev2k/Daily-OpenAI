$ErrorActionPreference = 'Stop'
$Container='relab-pgvis-20261029'
& docker version --format '{{.Server.Version}}' | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Start Docker Desktop first' }
$existing=& docker container inspect -f '{{.State.Running}}' $Container 2>$null
if ($LASTEXITCODE -ne 0) {
  & docker run -d --name $Container -e POSTGRES_USER=lab -e POSTGRES_PASSWORD=labpass -e POSTGRES_DB=labdb postgres:16.6 | Out-Null
  if ($LASTEXITCODE -ne 0) { throw 'Could not start postgres:16.6' }
} elseif (($existing -join '').Trim() -ne 'true') {
  & docker start $Container | Out-Null
  if ($LASTEXITCODE -ne 0) { throw 'Could not restart lab container' }
}
for ($i=0;$i -lt 45;$i++) {
  & docker exec $Container pg_isready -U lab -d labdb 2>$null | Out-Null
  if ($LASTEXITCODE -eq 0) { Write-Host 'Lab database ready'; exit 0 }
  Start-Sleep -Seconds 1
}
throw 'Database not ready after 45 seconds'
