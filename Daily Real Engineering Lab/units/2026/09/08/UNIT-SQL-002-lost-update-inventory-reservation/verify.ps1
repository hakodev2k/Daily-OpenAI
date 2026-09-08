$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) { exit 1 }
$text = $output -join "`n"
if ($text -notmatch "SUCCESS_COUNT=1") { Write-Error "FAIL: exactly one reservation should succeed."; exit 1 }
if ($text -notmatch "FINAL_QUANTITY=3") { Write-Error "FAIL: final quantity should be 3."; exit 1 }
Write-Host "PASS: inventory invariant is enforced atomically by the database."
