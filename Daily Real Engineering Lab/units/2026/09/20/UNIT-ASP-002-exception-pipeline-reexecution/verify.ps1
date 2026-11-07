$ErrorActionPreference = 'Stop'
$base = 'http://127.0.0.1:5188'
$proc = Start-Process dotnet -ArgumentList 'run','--project','./starter/ExceptionPipelineLab.csproj','--urls',$base -PassThru -WindowStyle Hidden
try {
  $ready = $false
  for ($i = 0; $i -lt 40; $i++) {
    try { Invoke-RestMethod "$base/tickets/ok" | Out-Null; $ready = $true; break } catch { Start-Sleep -Milliseconds 250 }
  }
  if (-not $ready) { throw 'Lab server did not become ready.' }
  Invoke-RestMethod -Method Delete "$base/__lab/audits" | Out-Null

  $okId = 'verify-ok'
  Invoke-RestMethod "$base/tickets/ok" -Headers @{ 'X-Lab-Request-Id' = $okId } | Out-Null
  $okCount = Invoke-RestMethod "$base/__lab/audits/$okId"

  $failId = 'verify-fail'
  try { Invoke-WebRequest "$base/tickets/fail" -Headers @{ 'X-Lab-Request-Id' = $failId } | Out-Null } catch { }
  $failCount = Invoke-RestMethod "$base/__lab/audits/$failId"

  Write-Host "success-audit-count=$okCount"
  Write-Host "failure-audit-count=$failCount"
  if ($okCount -eq 1 -and $failCount -eq 1) { Write-Host 'VERIFY_PASS'; exit 0 }
  Write-Error 'VERIFY_FAIL: each logical request must produce exactly one audit entry.'
  exit 1
}
finally {
  if ($proc -and -not $proc.HasExited) { Stop-Process -Id $proc.Id -Force }
}
