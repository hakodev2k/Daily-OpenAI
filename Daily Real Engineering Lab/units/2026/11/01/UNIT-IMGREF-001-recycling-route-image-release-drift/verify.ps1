$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project './starter/FleetDeployLab.csproj' -- verify
    if ($LASTEXITCODE -ne 0) { throw "Learner starter verification failed ($LASTEXITCODE)." }
}
finally { Pop-Location }
