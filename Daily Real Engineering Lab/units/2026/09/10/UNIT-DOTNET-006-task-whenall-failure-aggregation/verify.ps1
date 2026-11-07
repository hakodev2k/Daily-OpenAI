$ErrorActionPreference = 'Stop'

$output = dotnet run --project starter/BatchFailureLab.csproj 2>&1 | Out-String
$output | Write-Host

if ($LASTEXITCODE -ne 0) {
    throw "Learner starter exited with code $LASTEXITCODE"
}

if ($output -notmatch 'FAULTED_TASKS=2') {
    throw 'Verification expected exactly two faulted dependency tasks.'
}

if ($output -notmatch 'SUCCESSFUL_TASKS=1') {
    throw 'Verification expected the successful dependency to remain successful.'
}

if ($output -notmatch 'REPORTED_FAILURES=2') {
    throw 'Learner code must report both dependency failures.'
}

if ($output -notmatch 'TaxApi: unavailable') {
    throw 'TaxApi failure was not reported.'
}

if ($output -notmatch 'BenefitsApi: unavailable') {
    throw 'BenefitsApi failure was not reported.'
}

if ($output -notmatch 'FIX_VERIFIED: all dependency failures reported') {
    throw 'Expected fixed-state marker was not emitted.'
}

Write-Host 'VERIFIED: learner-editable starter reports all failures without regressing the successful dependency.'
