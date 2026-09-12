$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/WebhookLab.csproj'
$stdout = Join-Path $env:TEMP 'unit-aspnet-006-repro.out.log'
$stderr = Join-Path $env:TEMP 'unit-aspnet-006-repro.err.log'
$env:ASPNETCORE_URLS = 'http://127.0.0.1:5066'

$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--no-launch-profile') -PassThru -RedirectStandardOutput $stdout -RedirectStandardError $stderr
try {
    $ready = $false
    for ($i = 0; $i -lt 40; $i++) {
        Start-Sleep -Milliseconds 250
        try {
            Invoke-WebRequest 'http://127.0.0.1:5066/' -SkipHttpErrorCheck | Out-Null
            $ready = $true
            break
        } catch {}
    }
    if (-not $ready) { throw 'Starter service did not become ready.' }

    $body = '{"orderId":"ORD-42","status":"Paid"}'
    $response = Invoke-WebRequest 'http://127.0.0.1:5066/webhooks/order' -Method Post -ContentType 'application/json' -Body $body -SkipHttpErrorCheck

    if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 300) {
        throw "Reproduction failed: starter unexpectedly returned HTTP $($response.StatusCode)."
    }

    $log = if (Test-Path $stdout) { Get-Content $stdout -Raw } else { '' }
    if ($log -notmatch 'AUDIT BODY:.*ORD-42') {
        throw 'Expected middleware audit log to contain ORD-42.'
    }

    Write-Host "PASS: reproduced downstream failure after middleware captured the body. HTTP $($response.StatusCode)."
}
finally {
    if ($process -and -not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
