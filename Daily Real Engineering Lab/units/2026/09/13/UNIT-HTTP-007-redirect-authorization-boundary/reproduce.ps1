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

if ($exitCode -eq 0) {
    throw 'Expected starter to reproduce the failure, but it exited successfully.'
}

if ($output -notmatch 'FinalStatus=Unauthorized') {
    throw 'Expected FinalStatus=Unauthorized was not observed.'
}

Write-Host 'Reproduction confirmed: redirected request ends with Unauthorized.'
exit 0
