$ErrorActionPreference='Stop'
$out = dotnet run --project "$PSScriptRoot/starter/TraceLab.csproj"
$out | Write-Host
$line = $out | Where-Object { $_ -like 'slow-all=*' }
if ($line -notmatch 'slow-all=(\d+) slow-kept=(\d+)') { throw 'Không đọc được evidence.' }
if ([int]$Matches[1] -le 0 -or [int]$Matches[2] -ne 0) { throw 'Starter không reproduce đúng telemetry blind spot.' }
Write-Host 'REPRODUCED: toàn hệ thống có slow requests nhưng retained traces không chứa slow sample.'