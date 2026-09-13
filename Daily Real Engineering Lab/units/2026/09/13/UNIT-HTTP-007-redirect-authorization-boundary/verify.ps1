$ErrorActionPreference = 'Stop'

Push-Location "$PSScriptRoot/starter"
try {
    $output = dotnet run 2>&1 | Out-String
    $exitCode = $LASTEXITCODE
}
finally {
    Pop-Location
}

Write-Host $output

if ($exitCode -ne 0) {
    throw "Learner code still fails. Exit code: $exitCode"
}

if ($output -notmatch 'FinalStatus=OK') {
    throw 'Expected FinalStatus=OK was not observed.'
}

Write-Host 'Verification passed: learner code reaches the protected resource successfully.'
exit 0
