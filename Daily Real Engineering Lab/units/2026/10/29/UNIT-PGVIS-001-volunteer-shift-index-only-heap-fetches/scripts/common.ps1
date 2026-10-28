$ErrorActionPreference = 'Stop'
$Container = 'relab-pgvis-20261029'
$Root = Split-Path -Parent $PSScriptRoot
function Assert-LabContainer {
  & docker exec $Container pg_isready -U lab -d labdb | Out-Null
  if ($LASTEXITCODE -ne 0) { throw 'Postgres not ready; run ./setup.ps1' }
}
function Invoke-LabSqlFile([string]$RelativePath) {
  $path=Join-Path $Root $RelativePath
  if (-not (Test-Path $path)) { throw "Missing SQL: $path" }
  $name=Split-Path -Leaf $path
  & docker cp $path "${Container}:/tmp/$name" | Out-Null
  if ($LASTEXITCODE -ne 0) { throw "docker cp failed: $RelativePath" }
  & docker exec $Container psql -X -v ON_ERROR_STOP=1 -U lab -d labdb -f "/tmp/$name"
  if ($LASTEXITCODE -ne 0) { throw "psql failed: $RelativePath" }
}
function Get-LabCount {
  $q="SELECT count(*) FROM volunteer_shift_assignments WHERE shift_date=DATE '2026-10-29' AND status='CONFIRMED';"
  $raw=& docker exec $Container psql -X -q -t -A -v ON_ERROR_STOP=1 -U lab -d labdb -c $q
  if ($LASTEXITCODE -ne 0) { throw 'Count failed' }
  return [int](($raw -join '').Trim())
}
function Get-LabPlan {
  $q="EXPLAIN (ANALYZE, BUFFERS, FORMAT JSON) SELECT volunteer_id FROM volunteer_shift_assignments WHERE shift_date=DATE '2026-10-29' AND status='CONFIRMED';"
  $raw=& docker exec -e "PGOPTIONS=-c enable_seqscan=off -c enable_bitmapscan=off" $Container psql -X -q -t -A -v ON_ERROR_STOP=1 -U lab -d labdb -c $q
  if ($LASTEXITCODE -ne 0) { throw 'EXPLAIN failed' }
  $parsed=($raw -join "`n") | ConvertFrom-Json
  $plan=@($parsed)[0].Plan
  if ($null -eq $plan) { throw 'Missing EXPLAIN Plan' }
  if ($plan.'Node Type' -ne 'Index Only Scan') { throw "Expected Index Only Scan, got $($plan.'Node Type')" }
  return [pscustomobject]@{ NodeType=[string]$plan.'Node Type'; HeapFetches=[int]$plan.'Heap Fetches'; ActualRows=[int]$plan.'Actual Rows' }
}
