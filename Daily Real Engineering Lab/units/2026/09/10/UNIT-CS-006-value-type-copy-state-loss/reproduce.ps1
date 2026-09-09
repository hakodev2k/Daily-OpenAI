$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

$output = & dotnet run --project (Join-Path $root 'starter/RateCounterLab.csproj') 2>&1
if ($LASTEXITCODE -ne 0) {
    $output | ForEach-Object { Write-Host $_ }
    throw "Starter application failed to run."
}

$text = ($output -join [Environment]::NewLine)
Write-Host $text

if ($text -notmatch 'Iteration=1 ObservedCount=1') {
    throw "Expected the first local update to be observable."
}

if ($text -notmatch 'Iteration=3 ObservedCount=1') {
    throw "Expected each starter iteration to observe the same local incremented value."
}

if ($text -notmatch 'PersistedCount=0') {
    throw "Starter no longer reproduces the intended persisted-state symptom."
}

Write-Host "REPRODUCED: local updates are observed, but persisted state remains unchanged."
