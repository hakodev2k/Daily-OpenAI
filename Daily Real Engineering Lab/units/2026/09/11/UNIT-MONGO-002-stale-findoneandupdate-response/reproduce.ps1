$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    $output = dotnet run --project ./starter/Lab.csproj -- reproduce 2>&1
    $exit = $LASTEXITCODE
    $output | ForEach-Object { Write-Host $_ }

    if ($exit -ne 0) {
        throw "Starter did not reproduce the expected symptom. Exit code: $exit"
    }

    $text = $output -join "`n"
    if ($text -notmatch 'BEFORE_STATUS=Pending' -or
        $text -notmatch 'RETURNED_STATUS=Pending' -or
        $text -notmatch 'STORED_STATUS=Ready') {
        throw 'Expected evidence was not observed.'
    }

    Write-Host 'REPRODUCED: persisted state changed, but the returned document still shows the pre-update state.'
}
finally {
    Pop-Location
}
