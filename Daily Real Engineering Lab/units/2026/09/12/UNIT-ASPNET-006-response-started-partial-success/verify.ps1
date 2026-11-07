$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$failHeaders = Join-Path $env:TEMP 'unit-aspnet-006-verify-fail-headers.txt'
$failBody = Join-Path $env:TEMP 'unit-aspnet-006-verify-fail-body.txt'
$okHeaders = Join-Path $env:TEMP 'unit-aspnet-006-verify-ok-headers.txt'
$okBody = Join-Path $env:TEMP 'unit-aspnet-006-verify-ok-body.txt'
Remove-Item $failHeaders,$failBody,$okHeaders,$okBody -ErrorAction SilentlyContinue

$proc = Start-Process dotnet -ArgumentList @('run','--project', (Join-Path $root 'starter/Starter.csproj')) -PassThru -WindowStyle Hidden
try {
    $ready = $false
    for ($i = 0; $i -lt 40; $i++) {
        try {
            $null = Invoke-WebRequest 'http://127.0.0.1:5062/health' -UseBasicParsing -TimeoutSec 1
            $ready = $true
            break
        } catch { Start-Sleep -Milliseconds 250 }
    }
    if (-not $ready) { throw 'Learner starter service did not become ready.' }

    & curl.exe -sS -D $failHeaders -o $failBody 'http://127.0.0.1:5062/reports/orders.csv?fail=true' 2>$null
    $failStatus = (Get-Content $failHeaders | Select-Object -First 1)
    $failRows = @((Get-Content $failBody -ErrorAction SilentlyContinue) | Where-Object { $_ -match '^ORD-' }).Count

    if ($failStatus -match ' 200 ') { throw "Failure path still advertises success: $failStatus" }
    if ($failRows -gt 0) { throw "Failure path still exposes partial CSV rows: $failRows" }

    & curl.exe -sS -D $okHeaders -o $okBody 'http://127.0.0.1:5062/reports/orders.csv?fail=false' 2>$null
    $okStatus = (Get-Content $okHeaders | Select-Object -First 1)
    $okRows = @((Get-Content $okBody) | Where-Object { $_ -match '^ORD-' }).Count

    if ($okStatus -notmatch ' 200 ') { throw "Success path is not HTTP 200: $okStatus" }
    if ($okRows -ne 5) { throw "Success path must return 5 data rows, observed $okRows." }

    Write-Host 'PASS: failure no longer masquerades as successful partial CSV, and normal export remains correct.'
}
finally {
    if ($proc -and -not $proc.HasExited) { Stop-Process -Id $proc.Id -Force }
    Remove-Item $failHeaders,$failBody,$okHeaders,$okBody -ErrorAction SilentlyContinue
}
