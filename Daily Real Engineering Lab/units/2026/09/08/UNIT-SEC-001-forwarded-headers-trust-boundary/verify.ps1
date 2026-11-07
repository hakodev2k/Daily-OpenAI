$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$process = Start-Process dotnet -ArgumentList @("run", "--project", $project, "--no-launch-profile") -PassThru -WindowStyle Hidden
try {
    Start-Sleep -Seconds 3

    $direct = Invoke-RestMethod -Uri "http://127.0.0.1:5074/whoami" -Headers @{ "X-Forwarded-For" = "10.23.4.5" }
    Write-Host "Direct request with client-supplied forwarding metadata: clientIp=$($direct.clientIp) internal=$($direct.internalRequest)"

    if ($direct.internalRequest) {
        Write-Error "FAIL: direct request can still elevate its trust classification using client-controlled forwarding metadata."
        exit 1
    }

    Write-Host "PASS: client-controlled forwarding metadata no longer grants internal classification."
}
finally {
    if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
