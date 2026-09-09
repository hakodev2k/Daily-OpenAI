$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = dotnet run --project "$root/starter/Starter.csproj"
$output | ForEach-Object { Write-Host $_ }
$zero = $output | Where-Object { $_ -like 'merchant=M-ZERO*' }
$low = $output | Where-Object { $_ -like 'merchant=M-LOW*' }
$none = $output | Where-Object { $_ -like 'merchant=M-NONE*' }
if ($zero -notmatch 'matches=1' -or $zero -notmatch 'resolved=0($|\s)') { throw 'M-ZERO phải giữ override 0 hợp lệ.' }
if ($low -notmatch 'resolved=0.75') { throw 'M-LOW phải giữ override 0.75.' }
if ($none -notmatch 'matches=0' -or $none -notmatch 'resolved=2.5') { throw 'M-NONE phải dùng fallback 2.5.' }
Write-Host 'VERIFIED: presence và value được phân biệt đúng, không regression các case còn lại.'
