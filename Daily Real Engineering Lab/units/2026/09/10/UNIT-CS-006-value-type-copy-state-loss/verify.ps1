$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

$output = & dotnet run --project (Join-Path $root 'starter/RateCounterLab.csproj') 2>&1
if ($LASTEXITCODE -ne 0) {
    $output | ForEach-Object { Write-Host $_ }
    throw "Learner-editable starter application failed to run."
}

$text = ($output -join [Environment]::NewLine)
Write-Host $text

if ($text -notmatch 'PersistedCount=3') {
    throw "Verification failed: the three updates were not retained in persisted state."
}

if ($text -notmatch 'Iteration=3 ObservedCount=3') {
    throw "Verification failed: updates do not accumulate consistently through the third iteration."
}

Write-Host "VERIFIED: all three updates are retained and observable from the store."
