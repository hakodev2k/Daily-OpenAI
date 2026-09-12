$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/WebhookLab.csproj'
$stdout = Join-Path $env:TEMP 'unit-aspnet-006-verify.out.log'
$stderr = Join-Path $env:TEMP 'unit-aspnet-006-verify.err.log'
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
    if (-not $ready) { throw 'Learner service did not become ready.' }

    $body = '{"orderId":"ORD-42","status":"Paid"}'
    $response = Invoke-WebRequest 'http://127.0.0.1:5066/webhooks/order' -Method Post -ContentType 'application/json' -Body $body -SkipHttpErrorCheck

    if ($response.StatusCode -ne 200) {
        throw "Expected HTTP 200 after the fix, got $($response.StatusCode)."
    }
    if ($response.Content -notmatch 'ORD-42') {
        throw 'Response does not contain the expected order id.'
    }

    $log = if (Test-Path $stdout) { Get-Content $stdout -Raw } else { '' }
    if ($log -notmatch 'AUDIT BODY:.*ORD-42') { throw 'Middleware no longer captures the raw payload.' }
    if ($log -notmatch 'HANDLER orderId=ORD-42') { throw 'Business handler did not receive the expected payload.' }

    Write-Host 'PASS: learner-editable starter preserves both raw-body capture and downstream binding.'
}
finally {
    if ($process -and -not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
