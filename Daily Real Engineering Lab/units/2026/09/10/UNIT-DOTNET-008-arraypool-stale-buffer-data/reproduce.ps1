$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try {
    dotnet run -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Starter did not reproduce the intended symptom.' }
} finally {
    Pop-Location
}
