$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) { throw "Learner-edited starter did not run successfully." }
$text = $output -join "`n"
if ($text -notmatch "maxActive=1") { throw "FAIL: periodic executions still overlap." }
if ($text -notmatch "executions=([2-9]|[1-9][0-9]+)") { throw "FAIL: periodic execution no longer repeats." }
Write-Host "PASS: periodic work repeats without overlapping executions."
