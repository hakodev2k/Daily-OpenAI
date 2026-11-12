param([string]$Plan = "workspace/plan.json")
$ErrorActionPreference = "Stop"
$base = Split-Path -Parent $MyInvocation.MyCommand.Path
$p = Get-Content -LiteralPath (Join-Path $base $Plan) -Raw | ConvertFrom-Json
function Measure([string]$Name,[int]$Replicas,[int]$Pool,[int]$Max,[int]$Reserved,[int]$Headroom) {
  $budget = $Max - $Reserved
  $potential = $Replicas * $Pool
  [pscustomobject]@{Scenario=$Name; Replicas=$Replicas; PoolPerPod=$Pool; Potential=$potential; AppBudget=$budget; Headroom=($budget-$potential); Pass=($potential -le ($budget-$Headroom))}
}
@(
  (Measure "baseline" 4 30 240 40 20)
  (Measure "burst" 18 30 240 40 20)
  (Measure "candidate" ([int]$p.maxReplicas) ([int]$p.maxPoolPerPod) ([int]$p.dbMaxConnections) ([int]$p.reservedConnections) ([int]$p.requiredHeadroom))
) | Format-Table -AutoSize
