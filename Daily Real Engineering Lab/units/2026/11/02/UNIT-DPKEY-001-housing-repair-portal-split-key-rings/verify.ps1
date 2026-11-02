$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/DataProtectionLab.csproj -- --verify
    if ($LASTEXITCODE -ne 0) { throw "learner verification failed: exit code $LASTEXITCODE" }
} finally {
    Pop-Location
}
