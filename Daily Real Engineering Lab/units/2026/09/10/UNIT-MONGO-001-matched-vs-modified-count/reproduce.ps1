$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    $output = dotnet run --project ./starter/Starter.csproj 2>&1 | Out-String
    Write-Host $output

    $updates = @($output -split "`r?`n" | Where-Object { $_ -like 'UPDATE *' })
    if ($updates.Count -ne 3) {
        throw "Expected exactly 3 UPDATE lines, got $($updates.Count)."
    }

    if ($updates[0] -notmatch 'matched=1 modified=1 api=OK') {
        throw 'Initial changing update did not produce the expected baseline.'
    }

    if ($updates[1] -notmatch 'matched=1 modified=0 api=NOT_FOUND') {
        throw 'Starter symptom was not reproduced: expected existing no-op retry to be reported as NOT_FOUND.'
    }

    if ($updates[2] -notmatch 'matched=0 modified=0 api=NOT_FOUND') {
        throw 'Missing-document control case did not produce NOT_FOUND.'
    }

    Write-Host 'REPRODUCED: existing document + no-op retry is incorrectly reported as NOT_FOUND.'
}
finally {
    Pop-Location
}
