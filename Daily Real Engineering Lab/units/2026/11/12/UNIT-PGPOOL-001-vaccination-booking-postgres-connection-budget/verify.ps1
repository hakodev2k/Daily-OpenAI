$ErrorActionPreference = "Stop"
$base = Split-Path -Parent $MyInvocation.MyCommand.Path
$p = Get-Content -LiteralPath (Join-Path $base "workspace/plan.json") -Raw | ConvertFrom-Json
foreach ($f in @("maxReplicas","maxPoolPerPod","dbMaxConnections","reservedConnections","requiredHeadroom","admissionLimitPerPod")) {
  if ($null -eq $p.$f -or [double]$p.$f -ne [int]$p.$f -or [int]$p.$f -le 0) { throw "Invalid positive integer: $f" }
}
if ([int]$p.dbMaxConnections -ne 240 -or [int]$p.reservedConnections -ne 40) { throw "Shared DB constraints are fixed" }
if ([int]$p.requiredHeadroom -lt 20 -or [int]$p.maxReplicas -lt 12) { throw "Headroom or resilience constraint violated" }
if ([int]$p.admissionLimitPerPod -gt [int]$p.maxPoolPerPod * 6) { throw "Admission bound too permissive" }
$potential = [long]$p.maxReplicas * [long]$p.maxPoolPerPod
$limit = [long]$p.dbMaxConnections - [long]$p.reservedConnections - [long]$p.requiredHeadroom
if ($potential -gt $limit) { throw "FAIL: potential $potential exceeds safe limit $limit" }
Write-Host "PASS: potential=$potential <= safeLimit=$limit; benchmark SLO before production"
