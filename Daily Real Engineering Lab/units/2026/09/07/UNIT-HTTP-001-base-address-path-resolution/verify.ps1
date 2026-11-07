$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"

dotnet restore $project | Out-Null
$output = dotnet run --project $project
Write-Host $output

$uriMatch = $output | Select-String '^ObservedRequestUri=(.+)$'
$statusMatch = $output | Select-String '^StatusCode=(\d+)$'

if (-not $uriMatch -or -not $statusMatch) {
    Write-Error "FAIL: expected diagnostic output was not found."
    exit 1
}

$uri = $uriMatch.Matches.Groups[1].Value
$status = [int]$statusMatch.Matches.Groups[1].Value

if ($uri -ne "https://example.test/api/orders/42") {
    Write-Error "FAIL: request URI is '$uri'."
    exit 1
}

if ($status -ne 200) {
    Write-Error "FAIL: expected status 200 but got $status."
    exit 1
}

Write-Host "PASS: learner-editable HTTP client now targets the expected endpoint."
