$ErrorActionPreference = 'Stop'
if ($env:OS -ne 'Windows_NT') { throw 'Lab này cần Windows và cmd.exe.' }
if ($PSVersionTable.PSVersion.Major -lt 5) { throw 'Yêu cầu Windows PowerShell 5.1 hoặc PowerShell 7.' }
if (-not (Get-Command cmd.exe -ErrorAction SilentlyContinue)) { throw 'Không tìm thấy cmd.exe.' }
Write-Output "READY: $($PSVersionTable.PSVersion) / Windows / cmd.exe"
