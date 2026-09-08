$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) {
    Write-Error "FAIL: learner-edited starter did not run successfully."
    exit 1
}

$text = $output -join "
"
if ($text -notmatch "HEALTHY_RESULT=completed") {
    Write-Error "FAIL: healthy work still cannot complete after downstream failures."
    exit 1
}
if ($text -notmatch "FINAL permits=3") {
    Write-Error "FAIL: concurrency capacity was not fully restored."
    exit 1
}

Write-Host "PASS: failure paths preserve concurrency capacity and healthy work continues."
