$ErrorActionPreference = 'Stop'

Push-Location "$PSScriptRoot/starter"
try {
    dotnet run -- --verify
    if ($LASTEXITCODE -ne 0) {
        throw "Verification failed. The learner-editable starter still leaks request context. Exit code: $LASTEXITCODE"
    }
}
finally {
    Pop-Location
}
