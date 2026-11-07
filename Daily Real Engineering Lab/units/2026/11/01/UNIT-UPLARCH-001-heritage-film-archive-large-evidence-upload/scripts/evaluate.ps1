param(
  [Parameter(Mandatory=$true)][string]$PlanPath,
  [Parameter(Mandatory=$true)][string]$ConstraintsPath
)
$ErrorActionPreference = 'Stop'
$p = Get-Content -LiteralPath $PlanPath -Raw | ConvertFrom-Json
$c = Get-Content -LiteralPath $ConstraintsPath -Raw | ConvertFrom-Json
$issues = New-Object 'System.Collections.Generic.List[string]'
$strategy = [string]$p.strategy
$instances = [int]$p.instances
if ($instances -lt 1 -or $instances -gt [int]$c.maxInstances) {
  $issues.Add('instances nằm ngoài giới hạn mô hình.')
}
$safeInstances = [Math]::Max(1, $instances)
$activePerInstance = [Math]::Ceiling([double]$c.concurrentUploads / $safeInstances)
$memoryMiB = 0.0
$throughputMiBs = 0.0
switch ($strategy) {
  'buffered-proxy' {
    $memoryMiB = [double]$c.appBaseMemoryMiB + $activePerInstance * [double]$c.maxFileMiB
    $throughputMiBs = [double]$c.proxyOutboundMiBPerSecondPerInstance * $safeInstances
  }
  'streamed-proxy' {
    $memoryMiB = [double]$c.appBaseMemoryMiB + $activePerInstance * [double]$c.perStreamBufferMiB
    $throughputMiBs = [double]$c.proxyOutboundMiBPerSecondPerInstance * $safeInstances
  }
  'direct-block-upload' {
    $memoryMiB = [double]$c.appBaseMemoryMiB
    $throughputMiBs = [double]$c.directBlobAggregateMiBPerSecond
  }
  default {
    $issues.Add('strategy không hợp lệ.')
    $throughputMiBs = 1.0
  }
}
$totalMiB = [double]$c.maxFileMiB * [double]$c.concurrentUploads
$durationSeconds = [Math]::Ceiling($totalMiB / $throughputMiBs)
$computeCost = [double]$c.monthlyInstanceCostUnits * $safeInstances
if ($memoryMiB -gt [double]$c.appInstanceMemoryBudgetMiB) { $issues.Add('Memory budget vượt giới hạn mỗi instance.') }
if ($durationSeconds -gt [double]$c.maxCompletionSeconds) { $issues.Add('Thời gian hoàn tất vượt SLO.') }
if ($computeCost -gt [double]$c.monthlyComputeBudgetUnits) { $issues.Add('Compute budget vượt giới hạn.') }
$requiredControls = @('authAtInitiation','tenantIsolation','integrityAtCommit','resumable','malwareBeforePublish','auditTrail','abandonedCleanup','downloadAuthorization','uploadSizeLimit','idempotentFinalize')
foreach ($name in $requiredControls) {
  if ($null -eq $p.controls -or $p.controls.$name -ne $true) {
    $issues.Add(('Thiếu control: ' + $name))
  }
}
if ($strategy -eq 'direct-block-upload') {
  if ($null -eq $p.controls -or $p.controls.leastPrivilegeToken -ne $true) {
    $issues.Add('Direct upload yêu cầu token giới hạn quyền.')
  }
  if ([double]$p.tokenTtlMinutes -le 0 -or [double]$p.tokenTtlMinutes -gt [double]$c.maxTokenTtlMinutes) {
    $issues.Add('TTL token direct upload không hợp lệ.')
  }
}
if ([string]$p.rationale -eq '' -or ([string]$p.rationale).Length -lt 80) { $issues.Add('Cần rationale ít nhất 80 ký tự.') }
if ([string]$p.failureRecovery -eq '' -or ([string]$p.failureRecovery).Length -lt 60) { $issues.Add('Cần failureRecovery ít nhất 60 ký tự.') }
if ([string]$p.tradeoffs -eq '' -or ([string]$p.tradeoffs).Length -lt 60) { $issues.Add('Cần tradeoffs ít nhất 60 ký tự.') }
$rejected = @($p.rejectedOptions | Where-Object { $null -ne $_ })
if ($rejected.Count -lt 2) {
  $issues.Add('Cần so sánh ít nhất hai phương án bị loại.')
} else {
  $names = @()
  foreach ($option in $rejected) {
    if ([string]$option.strategy -eq $strategy -or ([string]$option.reason).Length -lt 30) {
      $issues.Add('Phương án bị loại phải khác phương án chọn và có lý do >= 30 ký tự.')
    }
    $names += [string]$option.strategy
  }
  if (@($names | Select-Object -Unique).Count -lt 2) { $issues.Add('Hai phương án bị loại phải khác nhau.') }
}
[PSCustomObject]@{
  Passed = ($issues.Count -eq 0)
  Strategy = $strategy
  Instances = $instances
  EstimatedPeakMemoryMiBPerInstance = [Math]::Round($memoryMiB,1)
  EstimatedCompletionSeconds = $durationSeconds
  EstimatedMonthlyComputeUnits = $computeCost
  Issues = @($issues.ToArray())
}
