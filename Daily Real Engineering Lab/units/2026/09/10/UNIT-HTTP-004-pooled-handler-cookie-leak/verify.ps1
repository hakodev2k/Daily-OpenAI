$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
dotnet build $project
$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--no-build','--no-launch-profile') -PassThru
try {
  $ready = $false
  for ($i = 0; $i -lt 30; $i++) {
    Start-Sleep -Milliseconds 300
    try { $null = Invoke-WebRequest 'http://127.0.0.1:5098/partner/session' -UseBasicParsing; $ready = $true; break } catch {}
  }
  if (-not $ready) { throw 'Lab server did not start.' }
  $result = (Invoke-WebRequest 'http://127.0.0.1:5098/lab' -UseBasicParsing).Content
  Write-Host $result
  if ($result -ne 'SECOND_CALL_COOKIE=<none>') { throw 'Verification failed: outbound state still crosses the request boundary.' }
  Write-Host 'VERIFIED: second request is isolated and starter behavior still runs.'
}
finally { if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force } }
