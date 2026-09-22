$ErrorActionPreference='Stop'
$out = dotnet run --project "$PSScriptRoot/starter/TraceLab.csproj"
$out | Write-Host
$first = $out | Where-Object { $_ -like 'requests=*' }
$last = $out | Where-Object { $_ -like 'slow-all=*' }
if ($first -notmatch 'requests=(\d+) traces=(\d+)') { throw 'Không đọc được trace count.' }
$total=[int]$Matches[1]; $traces=[int]$Matches[2]
if ($last -notmatch 'slow-all=(\d+) slow-kept=(\d+)') { throw 'Không đọc được slow evidence.' }
$slowAll=[int]$Matches[1]; $slowKept=[int]$Matches[2]
if ($slowAll -le 0) { throw 'Dataset không có slow request.' }
if ($slowKept -le 0) { throw 'FAIL: retained telemetry vẫn không chứa slow request để điều tra.' }
if ($traces -ge $total) { throw 'FAIL: không được giải quyết bằng cách giữ 100% traffic.' }
Write-Host 'PASS: retained telemetry chứa slow evidence trong khi volume vẫn được giới hạn.'