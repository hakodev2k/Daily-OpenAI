$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project './starter/FleetDeployLab.csproj' -- reproduce
    if ($LASTEXITCODE -ne 0) { throw "Original symptom was not reproduced ($LASTEXITCODE)." }
}
finally { Pop-Location }
