$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore ./starter/PropInventory.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Pop-Location
}
