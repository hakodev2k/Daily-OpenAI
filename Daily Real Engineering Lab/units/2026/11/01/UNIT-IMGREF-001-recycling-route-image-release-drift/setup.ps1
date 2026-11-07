$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore './starter/FleetDeployLab.csproj'
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed ($LASTEXITCODE)." }
}
finally { Pop-Location }
