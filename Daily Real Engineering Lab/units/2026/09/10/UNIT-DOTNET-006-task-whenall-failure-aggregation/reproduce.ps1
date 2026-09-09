$ErrorActionPreference = 'Stop'

$output = dotnet run --project starter/BatchFailureLab.csproj 2>&1 | Out-String
$output | Write-Host

if ($LASTEXITCODE -ne 0) {
    throw "Starter process exited with code $LASTEXITCODE"
}

if ($output -notmatch 'FAULTED_TASKS=2') {
    throw 'Expected exactly two faulted dependency tasks.'
}

if ($output -notmatch 'REPORTED_FAILURES=1') {
    throw 'Expected starter to report only one failure.'
}

if ($output -notmatch 'EXPECTED_FAILURE: only 1 of 2 failures reported') {
    throw 'Intended starter symptom was not reproduced.'
}

Write-Host 'REPRODUCED: two dependency tasks failed but only one failure was reported.'
