$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot 'starter')
try {
  $output = & dotnet run --project GatewayLab.csproj 2>&1
  $exitCode = $LASTEXITCODE
  $output | ForEach-Object { Write-Output $_ }
  if ($exitCode -eq 0 -or (($output -join "`n") -notmatch 'FRAME_ERROR|ROUNDTRIP_OK=False')) { throw 'Không tái hiện được triệu chứng dự kiến.' }
  Write-Output 'REPRODUCED: starter không round-trip được hai frame.'
} finally { Pop-Location }
