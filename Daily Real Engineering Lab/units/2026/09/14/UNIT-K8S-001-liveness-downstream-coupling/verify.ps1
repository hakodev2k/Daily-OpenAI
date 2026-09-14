$ErrorActionPreference = 'Stop'
$baseUrl = 'http://127.0.0.1:5199'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
$stdout = Join-Path $env:TEMP 'unit-k8s-001-verify.out.log'
$stderr = Join-Path $env:TEMP 'unit-k8s-001-verify.err.log'

$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--urls',$baseUrl) -PassThru -RedirectStandardOutput $stdout -RedirectStandardError $stderr

try {
    $started = $false
    for ($i = 0; $i -lt 30; $i++) {
        Start-Sleep -Milliseconds 300
        try {
            Invoke-WebRequest "$baseUrl/health/live" -UseBasicParsing | Out-Null
            $started = $true
            break
        } catch { }
    }
    if (-not $started) { throw "Learner app did not become ready. See $stderr" }

    Invoke-WebRequest "$baseUrl/admin/dependency/false" -Method Post -UseBasicParsing | Out-Null

    $live = Invoke-WebRequest "$baseUrl/health/live" -UseBasicParsing
    $readyStatus = $null
    try { Invoke-WebRequest "$baseUrl/health/ready" -UseBasicParsing | Out-Null } catch { $readyStatus = $_.Exception.Response.StatusCode.value__ }

    if ($live.StatusCode -ne 200) { throw "Expected /health/live to stay 200 while the process is healthy; got $($live.StatusCode)." }
    if ($readyStatus -ne 503) { throw "Expected /health/ready to return 503 while dependency is unavailable; got $readyStatus." }

    Invoke-WebRequest "$baseUrl/admin/dependency/true" -Method Post -UseBasicParsing | Out-Null
    $readyRecovered = Invoke-WebRequest "$baseUrl/health/ready" -UseBasicParsing
    if ($readyRecovered.StatusCode -ne 200) { throw "Expected readiness to recover to 200; got $($readyRecovered.StatusCode)." }

    Write-Host 'VERIFIED: liveness remains healthy during dependency outage, readiness blocks traffic, and readiness recovers without process restart.'
}
finally {
    if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
