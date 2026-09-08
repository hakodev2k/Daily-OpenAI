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
if ($text -notmatch "operation-A: correlation=corr-A") {
    Write-Error "FAIL: operation A lost its intended correlation."
    exit 1
}
if ($text -notmatch "operation-B: correlation=<null>") {
    Write-Error "FAIL: operation B still observes stale ambient correlation."
    exit 1
}

Write-Host "PASS: correlation context is scoped correctly."
