$ErrorActionPreference = 'Stop'
$port = 5184
$project = Join-Path $PSScriptRoot 'starter/App.csproj'
$proc = Start-Process dotnet -ArgumentList @('run','--project',$project,'--urls',"http://127.0.0.1:$port") -PassThru -NoNewWindow
try {
    $base = "http://127.0.0.1:$port"
    for ($i=0; $i -lt 30; $i++) {
        try { Invoke-WebRequest "$base/health/live" -UseBasicParsing | Out-Null; break } catch { Start-Sleep -Milliseconds 200 }
    }
    $ready = (Invoke-WebRequest "$base/health/ready" -UseBasicParsing -SkipHttpErrorCheck).StatusCode
    $business = (Invoke-WebRequest "$base/catalog/count" -UseBasicParsing -SkipHttpErrorCheck).StatusCode
    Write-Host "ready=$ready business=$business"
    if ($ready -eq 200 -and $business -eq 503) { Write-Host 'Reproduced readiness mismatch.'; exit 0 }
    throw "Expected ready=200 and business=503, got ready=$ready business=$business"
}
finally { if (!$proc.HasExited) { Stop-Process -Id $proc.Id -Force } }
