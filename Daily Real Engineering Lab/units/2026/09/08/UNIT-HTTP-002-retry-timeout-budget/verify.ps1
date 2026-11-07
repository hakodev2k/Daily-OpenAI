$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) { exit 1 }
$text = $output -join "`n"
if ($text -notmatch "RESULT=overall-timeout") { Write-Error "FAIL: overall deadline was not enforced."; exit 1 }
Write-Host "PASS: the dependency call respects the overall latency budget."
