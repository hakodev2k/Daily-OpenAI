$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Inventory.Api.csproj'
$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--no-launch-profile') -PassThru -WindowStyle Hidden
try {
    Start-Sleep -Seconds 2
    Invoke-RestMethod -Method Post -Uri 'http://127.0.0.1:5088/dependency/down' | Out-Null

    $live = Invoke-WebRequest -UseBasicParsing -Uri 'http://127.0.0.1:5088/health/live' -SkipHttpErrorCheck
    $ready = Invoke-WebRequest -UseBasicParsing -Uri 'http://127.0.0.1:5088/health/ready' -SkipHttpErrorCheck

    if ($live.StatusCode -ne 200) {
        throw "Verification failed: /health/live should remain 200 while the process itself is healthy."
    }
    if ($ready.StatusCode -eq 200) {
        throw "Verification failed: /health/ready should report dependency outage."
    }

    Write-Host "Verification passed: liveness and readiness now express different operational contracts."
}
finally {
    if ($process -and -not $process.HasExited) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        $process.WaitForExit()
    }
}
