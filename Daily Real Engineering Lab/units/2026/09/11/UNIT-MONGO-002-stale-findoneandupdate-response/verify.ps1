$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    $output = dotnet run --project ./starter/Lab.csproj -- verify 2>&1
    $exit = $LASTEXITCODE
    $output | ForEach-Object { Write-Host $_ }

    if ($exit -ne 0) {
        throw "Verification failed. Exit code: $exit"
    }

    $text = $output -join "`n"
    if ($text -notmatch 'BEFORE_STATUS=Pending' -or
        $text -notmatch 'RETURNED_STATUS=Ready' -or
        $text -notmatch 'STORED_STATUS=Ready') {
        throw 'Returned and persisted post-update state are not both correct.'
    }

    $source = Get-Content ./starter/Program.cs -Raw
    if ($source -match 'Find\(.*\)\.SingleAsync\(\)' -and $source -match 'MarkReadyAsync') {
        Write-Host 'Review note: verify behavior manually if you added an extra read inside MarkReadyAsync.'
    }

    Write-Host 'VERIFIED: learner-editable starter returns the post-update state and persisted state remains correct.'
}
finally {
    Pop-Location
}
