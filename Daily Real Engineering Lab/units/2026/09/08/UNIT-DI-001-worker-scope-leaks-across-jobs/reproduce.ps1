$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"

$output = dotnet run --project $project
Write-Host $output

$contextIds = @($output | ForEach-Object {
    if ($_ -match 'ContextId=(\d+)$') { $Matches[1] }
})

if ($contextIds.Count -ne 3) {
    Write-Error "REPRODUCE FAIL: expected three processed jobs."
    exit 1
}

$unique = @($contextIds | Sort-Object -Unique)
if ($unique.Count -ne 1) {
    Write-Error "REPRODUCE FAIL: intended symptom was not reproduced; expected one shared ContextId."
    exit 1
}

Write-Host "REPRODUCED: all jobs share the same ContextId."
