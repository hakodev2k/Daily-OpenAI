param([ValidateSet("starter","solution")][string]$Project = "starter")
$ErrorActionPreference = "Stop"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$port = Get-Random -Minimum 21000 -Maximum 29000
$p = Start-Process dotnet -WorkingDirectory $here -ArgumentList @("run","--no-launch-profile","--project","$Project/Webhook.csproj","--urls","http://127.0.0.1:$port") -PassThru -NoNewWindow
try {
  $ready = $false
  for ($i=0; $i -lt 50; $i++) {
    Start-Sleep -Milliseconds 200
    try { Invoke-WebRequest -Uri "http://127.0.0.1:$port/" -SkipHttpErrorCheck -TimeoutSec 1 | Out-Null; $ready=$true; break } catch {}
  }
  if (-not $ready) { throw "Web server not ready" }
  $response = Invoke-WebRequest -Uri "http://127.0.0.1:$port/webhooks/customs" -Method Post -ContentType "application/json" -Body '{"Reference":"SHIP-42"}' -SkipHttpErrorCheck
  if ($response.StatusCode -ne 200) { throw "Expected 200, got $($response.StatusCode): $($response.Content)" }
  $obj = $response.Content | ConvertFrom-Json
  if ($obj.reference -ne "SHIP-42" -or $obj.auditLength -le 0) { throw "Response mismatch: $($response.Content)" }
  Write-Host "PASS: audit and endpoint received payload"
} finally { Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue }
