$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "../starter/Lab.csproj"

dotnet restore $project | Out-Null
$output = dotnet run --project $project
Write-Host $output

$itemsMatch = $output | Select-String '^Items=(\d+)$'
$missingMatch = $output | Select-String '^MissingFeatured=(\d+)$'
$commandsMatch = $output | Select-String '^Commands=(\d+)$'

if (-not $itemsMatch -or -not $missingMatch -or -not $commandsMatch) {
    Write-Error "FAIL: expected diagnostics were not found."
    exit 1
}

$items = [int]$itemsMatch.Matches.Groups[1].Value
$missing = [int]$missingMatch.Matches.Groups[1].Value
$commands = [int]$commandsMatch.Matches.Groups[1].Value

if ($items -ne 8 -or $missing -ne 0) {
    Write-Error "FAIL: navigation behavior regressed. Items=$items MissingFeatured=$missing"
    exit 1
}

if ($commands -gt 2) {
    Write-Error "FAIL: workload still uses too many database commands. Commands=$commands"
    exit 1
}

Write-Host "PASS: navigation behavior is preserved and database roundtrips are bounded."
