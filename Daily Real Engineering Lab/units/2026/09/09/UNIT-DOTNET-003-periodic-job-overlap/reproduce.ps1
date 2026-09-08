$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) { throw "Starter did not run successfully." }
$text = $output -join "`n"
if ($text -notmatch "maxActive=([2-9]|[1-9][0-9]+)") { throw "Expected overlapping executions were not reproduced." }
Write-Host "REPRODUCED: more than one periodic execution was active at the same time."
