$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project './starter/FleetDeployLab.csproj' -- inspect
    if ($LASTEXITCODE -ne 0) { throw "inspect failed ($LASTEXITCODE)." }
}
finally { Pop-Location }
