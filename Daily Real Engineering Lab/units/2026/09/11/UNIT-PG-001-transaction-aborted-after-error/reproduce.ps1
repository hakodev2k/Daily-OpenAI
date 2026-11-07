$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    $output = dotnet run --project ./starter/Starter.csproj 2>&1 | Out-String
    $output | Write-Host

    if ($LASTEXITCODE -ne 0) {
        throw 'Starter process failed unexpectedly.'
    }

    if ($output -notmatch 'DUPLICATE_CAUGHT sqlstate=23505') {
        throw 'Expected duplicate evidence was not observed.'
    }

    if ($output -notmatch 'COMMAND_FAILED sqlstate=25P02') {
        throw 'Expected failed-transaction evidence was not observed.'
    }

    if ($output -notmatch 'ABORTED_STATE_OBSERVED=true') {
        throw 'Starter did not demonstrate the intended transaction-state symptom.'
    }

    Write-Host 'REPRODUCE_PASS: intended starter failure observed.'
}
finally {
    Pop-Location
}
