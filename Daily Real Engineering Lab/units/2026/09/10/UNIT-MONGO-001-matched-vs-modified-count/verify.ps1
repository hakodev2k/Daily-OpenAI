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
        throw 'FAIL: changing update must remain successful.'
    }

    if ($updates[1] -notmatch 'matched=1 modified=0 api=OK') {
        throw 'FAIL: an existing document with an idempotent no-op update must be treated as successful.'
    }

    if ($updates[2] -notmatch 'matched=0 modified=0 api=NOT_FOUND') {
        throw 'FAIL: a genuinely missing document must still be reported as NOT_FOUND.'
    }

    Write-Host 'VERIFIED: update, idempotent retry, and missing-document semantics are all correct.'
}
finally {
    Pop-Location
}
