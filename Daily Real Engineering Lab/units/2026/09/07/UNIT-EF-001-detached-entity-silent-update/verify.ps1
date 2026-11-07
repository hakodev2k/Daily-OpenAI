$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"

dotnet restore $project | Out-Null
$output = dotnet run --project $project

Write-Host $output

$persistedMatch = $output | Select-String '^PersistedPrice=(.+)$'
$changedMatch = $output | Select-String '^ChangedRows=(\d+)$'

if (-not $persistedMatch -or -not $changedMatch) {
    Write-Error "FAIL: expected diagnostic output was not found."
    exit 1
}

$persisted = $persistedMatch.Matches.Groups[1].Value
$changed = [int]$changedMatch.Matches.Groups[1].Value

if ($persisted -ne "129") {
    Write-Error "FAIL: expected PersistedPrice=129 but got '$persisted'."
    exit 1
}

if ($changed -lt 1) {
    Write-Error "FAIL: SaveChangesAsync did not persist any changed row."
    exit 1
}

Write-Host "PASS: learner-editable starter path persisted the new price."
