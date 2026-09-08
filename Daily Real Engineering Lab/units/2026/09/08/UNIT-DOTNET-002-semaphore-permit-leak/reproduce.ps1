$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) {
    Write-Error "REPRODUCE FAIL: starter did not run successfully."
    exit 1
}

$text = $output -join "
"
if ($text -notmatch "AFTER_FAILURES permits=0") {
    Write-Error "REPRODUCE FAIL: expected available capacity to drop to zero."
    exit 1
}
if ($text -notmatch "HEALTHY_RESULT=timed-out-waiting-for-capacity") {
    Write-Error "REPRODUCE FAIL: expected healthy work to time out."
    exit 1
}

Write-Host "REPRODUCED: failures permanently consumed all available concurrency capacity."
