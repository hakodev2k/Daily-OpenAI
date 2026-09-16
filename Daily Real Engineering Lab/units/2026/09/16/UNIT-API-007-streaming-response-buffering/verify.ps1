$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/StreamingLab.csproj'
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }
$lines = @($output | ForEach-Object { $_.ToString() })
$firstReceived = [Array]::FindIndex($lines, [Predicate[string]]{ param($x) $x -match '^CLIENT_RECEIVED 1 ' })
$lastProduced = [Array]::FindLastIndex($lines, [Predicate[string]]{ param($x) $x -match '^PRODUCED 8$' })
$totalOk = $lines -contains 'TOTAL_RECEIVED 8'
if ($firstReceived -lt 0 -or $lastProduced -lt 0 -or -not $totalOk) { throw 'Output contract chưa đúng hoặc thiếu evidence.' }
if ($firstReceived -lt $lastProduced) { Write-Host 'VERIFIED: learner code phát dữ liệu tăng dần và vẫn trả đủ 8 records.'; exit 0 }
throw 'Chưa đạt streaming property: record đầu tiên vẫn chỉ xuất hiện sau khi producer hoàn tất.'