$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/RegexPerformanceLab.csproj'
$output = dotnet run --project $project --configuration Release 2>&1 | Out-String
$output | Write-Host

if ($LASTEXITCODE -ne 0) {
    throw "Learner project failed to run."
}

if ($output -notmatch 'VALID_RESULT=True') {
    throw "Regression: valid slug is no longer accepted."
}

if ($output -notmatch 'INVALID_RESULT=False') {
    throw "Regression: invalid slug is no longer rejected."
}

$match = [regex]::Match($output, 'ADVERSARIAL_TIMEOUTS=(\d+)')
if (-not $match.Success) {
    throw "Could not find ADVERSARIAL_TIMEOUTS in learner output."
}

$timeouts = [int]$match.Groups[1].Value
if ($timeouts -ne 0) {
    throw "Verification failed: adversarial validation still times out."
}

Write-Host "Verification passed: functional behavior is preserved and the checked near-match inputs no longer time out."
