$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try {
    dotnet run -- --verify
    if ($LASTEXITCODE -ne 0) { throw 'Learner implementation does not satisfy the isolation contract.' }
} finally {
    Pop-Location
}
