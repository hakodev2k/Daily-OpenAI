$ErrorActionPreference = 'Stop'
$port = 5185
$project = Join-Path $PSScriptRoot 'starter/App.csproj'
$proc = Start-Process dotnet -ArgumentList @('run','--project',$project,'--urls',"http://127.0.0.1:$port") -PassThru -NoNewWindow
try {
    $base = "http://127.0.0.1:$port"
    for ($i=0; $i -lt 30; $i++) { try { Invoke-WebRequest "$base/health/live" -UseBasicParsing | Out-Null; break } catch { Start-Sleep -Milliseconds 200 } }
    $earlyReady = (Invoke-WebRequest "$base/health/ready" -UseBasicParsing -SkipHttpErrorCheck).StatusCode
    $earlyBusiness = (Invoke-WebRequest "$base/catalog/count" -UseBasicParsing -SkipHttpErrorCheck).StatusCode
    if ($earlyReady -eq 200 -or $earlyBusiness -ne 503) { throw "Before warmup expected readiness != 200 and business=503, got readiness=$earlyReady business=$earlyBusiness" }
    Start-Sleep -Seconds 5
    $lateReady = (Invoke-WebRequest "$base/health/ready" -UseBasicParsing -SkipHttpErrorCheck).StatusCode
    $lateBusiness = (Invoke-WebRequest "$base/catalog/count" -UseBasicParsing -SkipHttpErrorCheck).StatusCode
    if ($lateReady -ne 200 -or $lateBusiness -ne 200) { throw "After warmup expected both 200, got readiness=$lateReady business=$lateBusiness" }
    Write-Host 'PASS: readiness follows service capability.'
}
finally { if (!$proc.HasExited) { Stop-Process -Id $proc.Id -Force } }
