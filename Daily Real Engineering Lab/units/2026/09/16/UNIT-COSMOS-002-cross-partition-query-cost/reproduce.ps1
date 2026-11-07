$ErrorActionPreference='Stop'
$output = dotnet run --project (Join-Path $PSScriptRoot 'starter/CosmosCostLab.csproj') 2>&1
$output | ForEach-Object { Write-Host $_ }
$lines=@($output | ForEach-Object { $_.ToString() })
if (($lines -contains 'PARTITIONS_SCANNED 12') -and ($lines -contains 'ITEMS_RETURNED 1') -and ($lines -contains 'RESULT ORD-07-013')) { Write-Host 'REPRODUCED: result đúng nhưng lookup chạm toàn bộ logical partitions.'; exit 0 }
throw 'Starter không reproduce evidence mong đợi.'