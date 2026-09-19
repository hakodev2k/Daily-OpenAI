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
  $id = 'repro-001'
  try { Invoke-WebRequest "$base/tickets/fail" -Headers @{ 'X-Lab-Request-Id' = $id } | Out-Null } catch { }
  $count = Invoke-RestMethod "$base/__lab/audits/$id"
  Write-Host "audit-count=$count"
  if ($count -eq 2) { Write-Host 'REPRODUCED'; exit 0 }
  throw "Expected 2 audit entries in starter, observed $count."
}
finally {
  if ($proc -and -not $proc.HasExited) { Stop-Process -Id $proc.Id -Force }
}
