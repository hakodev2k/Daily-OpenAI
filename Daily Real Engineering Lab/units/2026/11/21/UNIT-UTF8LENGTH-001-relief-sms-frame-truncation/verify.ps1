$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot 'starter')
try {
  $output = & dotnet run --project GatewayLab.csproj 2>&1
  if ($LASTEXITCODE -ne 0) { throw "Chạy starter thất bại: $($output -join ' ')" }
  $output | ForEach-Object { Write-Output $_ }
  if (($output -join "`n") -notmatch 'ROUNDTRIP_OK=True') { throw 'Round-trip chưa đạt.' }
  if (($output -join "`n") -match 'FRAME_ERROR') { throw 'Có lỗi đọc frame.' }
  Write-Output 'PASS: hai frame round-trip thành công trên starter đã chỉnh sửa.'
} finally { Pop-Location }
