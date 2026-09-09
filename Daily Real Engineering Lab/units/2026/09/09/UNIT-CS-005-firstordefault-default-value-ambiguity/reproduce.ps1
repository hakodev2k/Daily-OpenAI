$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = dotnet run --project "$root/starter/Starter.csproj"
$output | ForEach-Object { Write-Host $_ }
$line = $output | Where-Object { $_ -like 'merchant=M-ZERO*' }
if (-not $line) { throw 'Không tìm thấy evidence cho M-ZERO.' }
if ($line -notmatch 'matches=1' -or $line -notmatch 'raw=0' -or $line -notmatch 'resolved=2.5') {
    throw 'Starter không còn reproduce đúng symptom mong đợi.'
}
Write-Host 'REPRODUCED: record hợp lệ tồn tại nhưng business result đi vào fallback.'
