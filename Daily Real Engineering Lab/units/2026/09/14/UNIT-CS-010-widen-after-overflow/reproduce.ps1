$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try {
    $output = dotnet run --configuration Release
    $output | ForEach-Object { Write-Host $_ }

    if ($output -notmatch 'totalCents=605032704') {
        throw 'Starter did not reproduce the expected incorrect total.'
    }

    Write-Host 'Reproduction succeeded: a plausible but incorrect billing total was observed.'
}
finally {
    Pop-Location
}
