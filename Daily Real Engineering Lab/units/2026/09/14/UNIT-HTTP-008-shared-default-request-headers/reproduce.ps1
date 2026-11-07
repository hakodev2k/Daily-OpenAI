$ErrorActionPreference = 'Stop'

Push-Location "$PSScriptRoot/starter"
try {
    dotnet run
    if ($LASTEXITCODE -ne 0) {
        throw "Starter did not reproduce the expected incident. Exit code: $LASTEXITCODE"
    }
}
finally {
    Pop-Location
}
