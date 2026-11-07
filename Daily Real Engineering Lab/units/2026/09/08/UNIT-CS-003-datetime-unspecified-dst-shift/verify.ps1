$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) { exit 1 }

$text = $output -join "`n"
if ($text -notmatch "BEFORE_DST=2026-03-07T14:00:00Z") { exit 1 }
if ($text -notmatch "AFTER_DST=2026-03-09T13:00:00Z") { exit 1 }

Write-Host "PASS: timezone rules are applied correctly."
