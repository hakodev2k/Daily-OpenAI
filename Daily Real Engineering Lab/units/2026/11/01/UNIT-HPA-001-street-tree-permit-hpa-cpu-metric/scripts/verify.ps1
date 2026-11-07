$ErrorActionPreference = 'Stop'
$output = @(& (Join-Path $PSScriptRoot 'diagnose.ps1'))
$text = $output -join "`n"
Write-Output $text
if ($text -notmatch 'METRIC_READY:') { throw 'VERIFY_FAIL: CPU utilization chưa tính được.' }
if ($output -notcontains 'DESIRED_REPLICAS=3') {
    throw 'VERIFY_FAIL: snapshot không đề xuất 3 replicas theo target.'
}
Write-Output 'VERIFY_PASS: starter/deployment.yaml đã chỉnh sửa đáp ứng contract của fixture.'
