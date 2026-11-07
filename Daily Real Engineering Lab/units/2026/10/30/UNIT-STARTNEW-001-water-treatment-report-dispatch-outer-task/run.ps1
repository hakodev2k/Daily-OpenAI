$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet run --project (Join-Path $PSScriptRoot 'harness/LabChecks.csproj') --configuration Release
    exit $LASTEXITCODE
}
finally { Pop-Location }
