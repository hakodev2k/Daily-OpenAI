$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Inventory.Api.csproj'

function Start-ServiceProcess {
    $p = Start-Process dotnet -ArgumentList @('run','--project',$project,'--no-launch-profile') -PassThru -WindowStyle Hidden
    Start-Sleep -Seconds 2
    return $p
}

function Stop-ServiceProcess($process) {
    if ($process -and -not $process.HasExited) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        $process.WaitForExit()
    }
}

$restarts = 0
$process = $null
try {
    $process = Start-ServiceProcess
    Invoke-RestMethod -Method Post -Uri 'http://127.0.0.1:5088/dependency/down' | Out-Null

    for ($i = 1; $i -le 3; $i++) {
        try {
            Invoke-WebRequest -UseBasicParsing -Uri 'http://127.0.0.1:5088/health/live' -TimeoutSec 3 | Out-Null
            Write-Host "Probe $i unexpectedly succeeded."
        }
        catch {
            $restarts++
            Write-Host "Probe $i failed -> simulated restart #$restarts"
            Stop-ServiceProcess $process
            $process = Start-ServiceProcess
            Invoke-RestMethod -Method Post -Uri 'http://127.0.0.1:5088/dependency/down' | Out-Null
        }
    }

    if ($restarts -lt 2) {
        throw "Expected repeated restart behavior was not reproduced."
    }

    Write-Host "Reproduction succeeded: dependency outage caused $restarts simulated restarts."
}
finally {
    Stop-ServiceProcess $process
}
