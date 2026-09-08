$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) { exit 1 }
$text = $output -join "`n"
if ($text -notmatch "SUCCESS_COUNT=2") { Write-Error "Expected both reservations to report success."; exit 1 }
if ($text -notmatch "FINAL_QUANTITY=3") { Write-Error "Expected lost-update symptom with final quantity 3."; exit 1 }
Write-Host "REPRODUCED: two reservations succeeded while only one decrement is reflected."
