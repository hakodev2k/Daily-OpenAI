$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project './starter/SmartMeterLedger.csproj' --configuration Release -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw "Lab command failed with exit code $LASTEXITCODE" }
}
finally { Pop-Location }
