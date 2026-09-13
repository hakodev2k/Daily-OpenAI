$ErrorActionPreference = 'Stop'

Push-Location "$PSScriptRoot/starter"
try {
    $output = dotnet run --configuration Release
    $output | ForEach-Object { Write-Host $_ }

    if ($output -notmatch 'near-match: TIMEOUT') {
        throw 'Expected near-match input to hit the validation timeout in the starter state.'
    }

    Write-Host 'Reproduction succeeded: input-dependent validation latency was observed.'
}
finally {
    Pop-Location
}
