$ErrorActionPreference = 'Stop'
$baseUrl = 'http://127.0.0.1:5199'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
$stdout = Join-Path $env:TEMP 'unit-k8s-001.out.log'
$stderr = Join-Path $env:TEMP 'unit-k8s-001.err.log'

$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--urls',$baseUrl) -PassThru -RedirectStandardOutput $stdout -RedirectStandardError $stderr

try {
    $ready = $false
    for ($i = 0; $i -lt 30; $i++) {
        Start-Sleep -Milliseconds 300
        try {
            Invoke-WebRequest "$baseUrl/health/live" -UseBasicParsing | Out-Null
            $ready = $true
            break
        } catch { }
    }

    if (-not $ready) { throw "Starter app did not become ready. See $stderr" }

    Write-Host '== Baseline =='
    $liveBefore = Invoke-WebRequest "$baseUrl/health/live" -UseBasicParsing
    $readyBefore = Invoke-WebRequest "$baseUrl/health/ready" -UseBasicParsing
    Write-Host "live:  $($liveBefore.StatusCode)"
    Write-Host "ready: $($readyBefore.StatusCode)"

    Invoke-WebRequest "$baseUrl/admin/dependency/false" -Method Post -UseBasicParsing | Out-Null

    Write-Host '== Dependency unavailable, process still running =='
    $liveAfter = $null
    $readyAfter = $null
    try { Invoke-WebRequest "$baseUrl/health/live" -UseBasicParsing | Out-Null } catch { $liveAfter = $_.Exception.Response.StatusCode.value__ }
    try { Invoke-WebRequest "$baseUrl/health/ready" -UseBasicParsing | Out-Null } catch { $readyAfter = $_.Exception.Response.StatusCode.value__ }
    Write-Host "process exited: $($process.HasExited)"
    Write-Host "live:  $liveAfter"
    Write-Host "ready: $readyAfter"

    if ($process.HasExited) { throw 'Unexpected: starter process exited.' }
    if ($liveAfter -ne 503) { throw "Expected starter /health/live to return 503 during dependency outage, got $liveAfter." }
    if ($readyAfter -ne 503) { throw "Expected starter /health/ready to return 503 during dependency outage, got $readyAfter." }

    Write-Host 'REPRODUCED: a running process exposes a failing liveness signal during an external dependency outage.'
}
finally {
    if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
