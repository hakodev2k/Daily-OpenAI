# Mô hình local chỉ hỗ trợ cấu trúc YAML cố định của lab; không phải YAML parser hoặc HPA emulator tổng quát.
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$deployment = Get-Content -LiteralPath (Join-Path $root 'starter/deployment.yaml') -Raw
$hpa = Get-Content -LiteralPath (Join-Path $root 'starter/hpa.yaml') -Raw
$rows = @(Import-Csv -LiteralPath (Join-Path $root 'starter/pod-cpu.csv'))
$containers = @{}
$current = $null
$insideContainers = $false
$insideRequests = $false
foreach ($line in ($deployment -split '\r?\n')) {
    if ($line -match '^      containers:\s*$') { $insideContainers = $true; continue }
    if (-not $insideContainers) { continue }
    if ($line -match '^        - name:\s*([a-z0-9-]+)\s*$') {
        $current = $Matches[1]
        if ($containers.ContainsKey($current)) { throw "Duplicate container: $current" }
        $containers[$current] = $null
        $insideRequests = $false
        continue
    }
    if ($line -match '^            requests:\s*$') { $insideRequests = $true; continue }
    if ($line -match '^            [a-zA-Z]+:\s*$') { $insideRequests = $false; continue }
    if ($insideRequests -and $line -match '^              cpu:\s*"?([0-9]+)m"?\s*$') {
        $containers[$current] = [int]$Matches[1]
    }
}
if ($containers.Count -ne 2 -or -not $containers.ContainsKey('permit-api') -or -not $containers.ContainsKey('audit-agent')) {
    throw 'Fixture yêu cầu đúng hai containers: permit-api và audit-agent.'
}
$replicaMatch = [regex]::Match($deployment, '(?m)^  replicas:\s*(\d+)\s*$')
$minMatch = [regex]::Match($hpa, '(?m)^  minReplicas:\s*(\d+)\s*$')
$maxMatch = [regex]::Match($hpa, '(?m)^  maxReplicas:\s*(\d+)\s*$')
$targetMatch = [regex]::Match($hpa, '(?m)^          averageUtilization:\s*(\d+)\s*$')
if (-not ($replicaMatch.Success -and $minMatch.Success -and $maxMatch.Success -and $targetMatch.Success)) {
    throw 'Thiếu replica hoặc CPU utilization target trong fixture.'
}
$replicas = [int]$replicaMatch.Groups[1].Value
$minReplicas = [int]$minMatch.Groups[1].Value
$maxReplicas = [int]$maxMatch.Groups[1].Value
$target = [int]$targetMatch.Groups[1].Value
if ($replicas -ne 2 -or $minReplicas -ne 2 -or $maxReplicas -ne 8 -or $target -ne 70) {
    throw 'Không thay đổi replica bounds hoặc HPA target trong lab.'
}
foreach ($name in @('permit-api','audit-agent')) {
    if ($null -eq $containers[$name] -or [int]$containers[$name] -le 0) {
        Write-Output "METRIC_UNAVAILABLE: container=$name missing positive cpu request"
        return
    }
}
$pods = @($rows | Group-Object pod)
if ($pods.Count -ne $replicas) { throw 'Số Pod trong snapshot không khớp replicas.' }
$totalUsage = 0
$totalRequest = 0
foreach ($pod in $pods) {
    foreach ($name in @('permit-api','audit-agent')) {
        $matched = @($pod.Group | Where-Object { $_.container -eq $name })
        if ($matched.Count -ne 1) { throw "Thiếu hoặc trùng metric cho $($pod.Name)/$name" }
        $usage = [int]$matched[0].usageMilliCpu
        if ($usage -lt 0) { throw 'CPU usage không được âm.' }
        $totalUsage += $usage
        $totalRequest += [int]$containers[$name]
    }
}
$utilization = 100.0 * $totalUsage / $totalRequest
$rawDesired = [math]::Ceiling($replicas * $utilization / $target)
$desired = [int][math]::Min($maxReplicas, [math]::Max($minReplicas, $rawDesired))
Write-Output ("METRIC_READY: usage={0}m request={1}m utilization={2:F1}% target={3}%" -f $totalUsage,$totalRequest,$utilization,$target)
Write-Output "DESIRED_REPLICAS=$desired"
Write-Output 'MODEL_SCOPE: no tolerance, stabilization, readiness or missing-metrics adjustment'
