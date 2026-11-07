param(
    [Parameter(Mandatory = $true)][string]$PolicyPath,
    [switch]$Check
)
$ErrorActionPreference = 'Stop'
$policy = Get-Content -LiteralPath $PolicyPath -Raw | ConvertFrom-Json
$strategy = [string]$policy.partitionStrategy
$allowed = @('cooperative', 'region-day', 'voucher-shard', 'attempt-shard')
if ($strategy -notin $allowed) {
    throw "Unsupported partitionStrategy: $strategy"
}

function Get-VoucherShard {
    param([string]$VoucherId)
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $hash = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($VoucherId))
        return [int]($hash[0] % 8)
    }
    finally {
        $sha.Dispose()
    }
}

function Resolve-Partition {
    param(
        [string]$Strategy,
        [string]$VoucherId,
        [string]$CooperativeId,
        [string]$Region,
        [string]$Day,
        [int]$Attempt
    )
    switch ($Strategy) {
        'cooperative' { return ('coop:' + $CooperativeId) }
        'region-day' { return ('day:' + $Region + ':' + $Day) }
        'voucher-shard' { return ('shard:' + (Get-VoucherShard -VoucherId $VoucherId)) }
        'attempt-shard' { return ('shard:' + ($Attempt % 8)) }
    }
}

$counts = @{}
for ($i = 0; $i -lt 1000; $i++) {
    $voucherId = 'SEED-{0:D4}' -f $i
    $coop = if ($i -lt 900) { 'COOP-HOT' } else { 'COOP-{0:D2}' -f ($i % 10) }
    $bucket = Resolve-Partition -Strategy $strategy -VoucherId $voucherId -CooperativeId $coop -Region 'R1' -Day '2026-10-31' -Attempt $i
    if (-not $counts.ContainsKey($bucket)) { $counts[$bucket] = 0 }
    $counts[$bucket]++
}

$original = Resolve-Partition -Strategy $strategy -VoucherId 'SEED-0042' -CooperativeId 'COOP-HOT' -Region 'R1' -Day '2026-10-31' -Attempt 42
$replay = Resolve-Partition -Strategy $strategy -VoucherId 'SEED-0042' -CooperativeId 'COOP-HOT' -Region 'R1' -Day '2026-10-31' -Attempt 1001
$replayStable = $original -eq $replay
$pointReadFromVoucherId = $strategy -eq 'voucher-shard'
$maxBucket = [int](($counts.Values | Measure-Object -Maximum).Maximum)
$maxShare = [math]::Round($maxBucket / 1000.0, 3)
Write-Host ('strategy=' + $strategy)
Write-Host ('bucketCount=' + $counts.Count)
Write-Host ('maxBucketCount=' + $maxBucket)
Write-Host ('maxBucketShare=' + $maxShare.ToString([System.Globalization.CultureInfo]::InvariantCulture))
Write-Host ('replayStable=' + $replayStable)
Write-Host ('pointReadFromVoucherId=' + $pointReadFromVoucherId)
Write-Host 'Note: synthetic logical buckets, NOT actual Cosmos DB RU/s or physical partitions.'

if ($Check) {
    $failures = @()
    if ($maxShare -gt 0.22) { $failures += 'hotspot share exceeds 22 percent' }
    if (-not $replayStable) { $failures += 'duplicate voucher retry routes to a different partition' }
    if (-not $pointReadFromVoucherId) { $failures += 'voucher-ID-only point read cannot determine partition key' }
    if ($failures.Count -gt 0) {
        foreach ($failure in $failures) { Write-Host ('FAIL: ' + $failure) }
        exit 1
    }
    Write-Host 'PASS: all synthetic acceptance checks satisfied.'
}
