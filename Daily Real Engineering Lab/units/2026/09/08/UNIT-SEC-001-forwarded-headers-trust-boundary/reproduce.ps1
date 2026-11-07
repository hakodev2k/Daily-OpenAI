$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$process = Start-Process dotnet -ArgumentList @("run", "--project", $project, "--no-launch-profile") -PassThru -WindowStyle Hidden
try {
    Start-Sleep -Seconds 3
    $normal = Invoke-RestMethod -Uri "http://127.0.0.1:5074/whoami"
    Write-Host "Normal request: clientIp=$($normal.clientIp) internal=$($normal.internalRequest)"

    $spoofed = Invoke-RestMethod -Uri "http://127.0.0.1:5074/whoami" -Headers @{ "X-Forwarded-For" = "10.23.4.5" }
    Write-Host "Client-supplied metadata: clientIp=$($spoofed.clientIp) internal=$($spoofed.internalRequest)"

    if (-not $spoofed.internalRequest) {
        Write-Error "REPRODUCE FAIL: expected the starter to misclassify client-supplied metadata as internal."
        exit 1
    }

    Write-Host "REPRODUCED: untrusted request metadata changes the application's trust classification."
}
finally {
    if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
