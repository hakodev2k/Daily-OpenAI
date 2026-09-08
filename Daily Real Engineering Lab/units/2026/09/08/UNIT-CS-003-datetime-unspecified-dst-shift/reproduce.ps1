$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) {
    Write-Error "REPRODUCE FAIL: starter did not run successfully."
    exit 1
}

$text = $output -join "`n"
if ($text -notmatch "BEFORE_DST=2026-03-07T14:00:00Z") {
    Write-Error "REPRODUCE FAIL: baseline before DST did not match expected behavior."
    exit 1
}
if ($text -notmatch "AFTER_DST=2026-03-09T14:00:00Z") {
    Write-Error "REPRODUCE FAIL: expected the fixed-offset bug after DST."
    exit 1
}

Write-Host "REPRODUCED: fixed UTC-05:00 offset schedules New York 09:00 one hour late after DST starts."
