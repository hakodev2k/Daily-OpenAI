$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$headers = Join-Path $env:TEMP 'unit-aspnet-006-repro-headers.txt'
$body = Join-Path $env:TEMP 'unit-aspnet-006-repro-body.txt'
Remove-Item $headers,$body -ErrorAction SilentlyContinue

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
    if (-not $ready) { throw 'Starter service did not become ready.' }

    & curl.exe -sS -D $headers -o $body 'http://127.0.0.1:5062/reports/orders.csv?fail=true' 2>$null
    $statusLine = (Get-Content $headers | Select-Object -First 1)
    $content = Get-Content $body -Raw -ErrorAction SilentlyContinue
    $dataRows = @((Get-Content $body -ErrorAction SilentlyContinue) | Where-Object { $_ -match '^ORD-' }).Count

    if ($statusLine -notmatch ' 200 ') { throw "Expected starter to expose HTTP 200, got: $statusLine" }
    if ($dataRows -ge 5) { throw "Expected truncated CSV, but observed $dataRows data rows." }

    Write-Host "PASS: reproduced partial-success symptom. Status='$statusLine', dataRows=$dataRows"
    Write-Host 'Inspect the response lifecycle before changing the code.'
}
finally {
    if ($proc -and -not $proc.HasExited) { Stop-Process -Id $proc.Id -Force }
    Remove-Item $headers,$body -ErrorAction SilentlyContinue
}
