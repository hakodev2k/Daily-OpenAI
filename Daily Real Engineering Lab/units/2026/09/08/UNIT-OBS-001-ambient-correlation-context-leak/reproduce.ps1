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
if ($text -notmatch "operation-B: correlation=corr-A") {
    Write-Error "REPRODUCE FAIL: expected operation B to observe stale correlation."
    exit 1
}

Write-Host "REPRODUCED: ambient correlation leaked into the next logical operation."
