$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/DataProtectionLab.csproj -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw "starter reproduction contract failed: exit code $LASTEXITCODE" }
} finally {
    Pop-Location
}
