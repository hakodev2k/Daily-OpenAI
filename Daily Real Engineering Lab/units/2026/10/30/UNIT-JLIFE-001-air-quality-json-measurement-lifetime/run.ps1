$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet run --project (Join-Path $PSScriptRoot 'starter/AirQualityLab.csproj') --configuration Release -- check
    exit $LASTEXITCODE
}
finally { Pop-Location }
