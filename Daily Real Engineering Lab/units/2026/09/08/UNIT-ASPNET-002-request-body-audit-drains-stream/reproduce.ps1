$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$process = Start-Process dotnet -ArgumentList @("run","--project",$project) -PassThru -WindowStyle Hidden
try {
    Start-Sleep -Seconds 2
    $body = '{"paymentId":"pay-0908","amount":149.50}'
    try {
        $response = Invoke-WebRequest -Uri "http://127.0.0.1:5086/webhooks/payment" -Method Post -ContentType "application/json" -Body $body
        Write-Error "REPRODUCE FAIL: expected starter failure but got HTTP $($response.StatusCode)."
        exit 1
    } catch {
        $status = [int]$_.Exception.Response.StatusCode
        if ($status -ne 400) {
            Write-Error "REPRODUCE FAIL: expected HTTP 400 but got $status."
            exit 1
        }
        Write-Host "REPRODUCED: valid webhook is rejected after audit middleware reads the body."
    }
}
finally {
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
}
