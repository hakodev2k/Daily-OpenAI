$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) {
    Write-Error "FAIL: learner-edited starter did not run successfully."
    exit 1
}

$text = $output -join "`n"
if ($text -notmatch "EFFECTIVE=https://payments.prod.example/") {
    Write-Error "FAIL: effective configuration still does not use production endpoint."
    exit 1
}
if ($text -match "EFFECTIVE=https://payments.staging.example/") {
    Write-Error "FAIL: stale staging override is still active."
    exit 1
}

Write-Host "PASS: effective configuration uses the intended production endpoint."
