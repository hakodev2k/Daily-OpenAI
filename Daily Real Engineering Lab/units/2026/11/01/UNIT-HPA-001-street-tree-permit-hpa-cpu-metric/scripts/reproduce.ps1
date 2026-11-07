$ErrorActionPreference = 'Stop'
$output = @(& (Join-Path $PSScriptRoot 'diagnose.ps1'))
$text = $output -join "`n"
Write-Output $text
if ($text -notmatch 'METRIC_UNAVAILABLE: container=audit-agent') {
    throw 'REPRODUCE_FAIL: Không thấy triệu chứng của starter.'
}
Write-Output 'REPRODUCE_PASS: đã tái hiện triệu chứng cấu hình ban đầu.'
