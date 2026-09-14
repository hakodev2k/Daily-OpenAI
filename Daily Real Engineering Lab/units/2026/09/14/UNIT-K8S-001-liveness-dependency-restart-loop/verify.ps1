$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Inventory.Api.csproj'

function Get-HttpStatus([string]$uri) {
    try {
        $response = Invoke-WebRequest -UseBasicParsing -Uri $uri -TimeoutSec 3
        return [int]$response.StatusCode
    }
    catch {
        if ($_.Exception.Response) {
            return [int]$_.Exception.Response.StatusCode
        }
        throw
    }
}

$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--no-launch-profile') -PassThru -WindowStyle Hidden
try {
    Start-Sleep -Seconds 2
    Invoke-RestMethod -Method Post -Uri 'http://127.0.0.1:5088/dependency/down' | Out-Null

    $liveStatus = Get-HttpStatus 'http://127.0.0.1:5088/health/live'
    $readyStatus = Get-HttpStatus 'http://127.0.0.1:5088/health/ready'

    if ($liveStatus -ne 200) {
        throw "Verification failed: /health/live should remain 200 while the process itself is healthy."
    }
    if ($readyStatus -eq 200) {
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
