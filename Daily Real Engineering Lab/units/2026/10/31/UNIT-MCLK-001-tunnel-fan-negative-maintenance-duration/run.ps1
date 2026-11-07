$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project './starter/ClockLab.csproj' --configuration Release -- demo
    if ($LASTEXITCODE -ne 0) { throw 'Demo run failed.' }
} finally { Pop-Location }
