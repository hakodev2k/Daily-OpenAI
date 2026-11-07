$ErrorActionPreference='Stop'
$output = dotnet run --project (Join-Path $PSScriptRoot 'starter/CosmosCostLab.csproj') 2>&1
$output | ForEach-Object { Write-Host $_ }
$lines=@($output | ForEach-Object { $_.ToString() })
$partitionLine=$lines | Where-Object { $_ -match '^PARTITIONS_SCANNED ' } | Select-Object -First 1
if (-not $partitionLine) { throw 'Thiếu partition evidence.' }
$count=[int]($partitionLine -replace '^PARTITIONS_SCANNED ','')
if (($count -eq 1) -and ($lines -contains 'ITEMS_RETURNED 1') -and ($lines -contains 'RESULT ORD-07-013')) { Write-Host 'VERIFIED: lookup giữ đúng result và chỉ chạm partition cần thiết.'; exit 0 }
throw 'Chưa đạt yêu cầu: giữ đúng result và thu hẹp lookup còn một logical partition.'