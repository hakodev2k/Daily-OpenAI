$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"

$output = dotnet run --project $project
Write-Host $output

$contextIds = @($output | ForEach-Object {
    if ($_ -match 'ContextId=(\d+)$') { $Matches[1] }
})

if ($contextIds.Count -ne 3) {
    Write-Error "FAIL: expected three processed jobs."
    exit 1
}

$unique = @($contextIds | Sort-Object -Unique)
if ($unique.Count -ne 3) {
    Write-Error "FAIL: expected one independent ContextId per job."
    exit 1
}

Write-Host "PASS: each job uses an independent scoped context."
