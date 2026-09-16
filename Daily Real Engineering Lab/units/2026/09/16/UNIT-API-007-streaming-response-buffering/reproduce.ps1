$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/StreamingLab.csproj'
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }
$lines = @($output | ForEach-Object { $_.ToString() })
$lastProduced = [Array]::FindLastIndex($lines, [Predicate[string]]{ param($x) $x -match '^PRODUCED ' })
$firstReceived = [Array]::FindIndex($lines, [Predicate[string]]{ param($x) $x -match '^CLIENT_RECEIVED ' })
if ($lastProduced -lt 0 -or $firstReceived -lt 0) { throw 'Không thu được evidence cần thiết.' }
if ($firstReceived -gt $lastProduced) { Write-Host 'REPRODUCED: client chỉ nhận dữ liệu sau khi producer hoàn tất batch.'; exit 0 }
throw 'Starter không còn reproduce symptom mong đợi.'