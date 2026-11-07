$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--no-launch-profile') -PassThru
try {
  $ready = $false
  for ($i = 0; $i -lt 30; $i++) {
    Start-Sleep -Milliseconds 300
    try { $null = Invoke-WebRequest 'http://127.0.0.1:5098/partner/session' -UseBasicParsing; $ready = $true; break } catch {}
  }
  if (-not $ready) { throw 'Lab server did not start.' }
  $result = (Invoke-WebRequest 'http://127.0.0.1:5098/lab' -UseBasicParsing).Content
  Write-Host $result
  if ($result -notmatch 'SECOND_CALL_COOKIE=LegacySession=tenant-a') { throw 'Expected starter symptom was not reproduced.' }
  Write-Host 'REPRODUCED: second client observed cookie state from the earlier call.'
}
finally { if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force } }
