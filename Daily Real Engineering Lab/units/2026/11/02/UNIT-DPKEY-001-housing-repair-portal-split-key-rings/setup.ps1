$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore ./starter/DataProtectionLab.csproj
    if ($LASTEXITCODE -ne 0) { throw "restore starter failed: exit code $LASTEXITCODE" }
} finally {
    Pop-Location
}
