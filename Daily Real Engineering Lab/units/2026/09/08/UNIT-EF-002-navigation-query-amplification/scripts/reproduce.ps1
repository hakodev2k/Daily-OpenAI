$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "../starter/Lab.csproj"

dotnet restore $project | Out-Null
$output = dotnet run --project $project
Write-Host $output

$items = [int](($output | Select-String '^Items=(\d+)$').Matches.Groups[1].Value)
$missing = [int](($output | Select-String '^MissingFeatured=(\d+)$').Matches.Groups[1].Value)
$commands = [int](($output | Select-String '^Commands=(\d+)$').Matches.Groups[1].Value)

if ($items -ne 8 -or $missing -ne 0) {
    Write-Error "REPRODUCE FAILED: functional baseline is not as expected."
    exit 1
}

if ($commands -lt 9) {
    Write-Error "REPRODUCE FAILED: intended query amplification was not observed. Commands=$commands"
    exit 1
}

Write-Host "REPRODUCED: functional output is correct but database command count grows with navigation size."
