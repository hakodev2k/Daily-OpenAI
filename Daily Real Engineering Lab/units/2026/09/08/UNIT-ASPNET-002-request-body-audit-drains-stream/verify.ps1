$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$process = Start-Process dotnet -ArgumentList @("run","--project",$project) -PassThru -WindowStyle Hidden
try {
    Start-Sleep -Seconds 2
    $body = '{"paymentId":"pay-0908","amount":149.50}'
    $response = Invoke-WebRequest -Uri "http://127.0.0.1:5086/webhooks/payment" -Method Post -ContentType "application/json" -Body $body
    if ($response.StatusCode -ne 200) {
        Write-Error "FAIL: expected HTTP 200."
        exit 1
    }
    $json = $response.Content | ConvertFrom-Json
    if ($json.paymentId -ne "pay-0908") {
        Write-Error "FAIL: wrong payload."
        exit 1
    }
    Write-Host "PASS"
}
finally {
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
}
