$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    $output = dotnet run --project ./starter/Starter.csproj 2>&1 | Out-String
    $output | Write-Host

    if ($LASTEXITCODE -ne 0) {
        throw 'Learner solution process failed.'
    }

    if ($output -match 'COMMAND_FAILED sqlstate=25P02') {
        throw 'Transaction still enters failed state after the handled duplicate.'
    }

    if ($output -notmatch 'BATCH_COMMITTED=true') {
        throw 'Batch did not report a successful commit.'
    }

    if ($output -notmatch 'ROW_COUNT=2') {
        throw 'Expected exactly two distinct customers after the batch.'
    }

    Write-Host 'VERIFY_PASS: duplicate policy and transaction behavior are correct.'
}
finally {
    Pop-Location
}
