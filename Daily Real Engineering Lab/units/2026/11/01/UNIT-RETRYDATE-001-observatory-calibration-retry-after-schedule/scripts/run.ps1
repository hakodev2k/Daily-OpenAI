$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..')
try {
    & dotnet run --project './starter/RetryPlannerLab.csproj' -- inspect
    exit $LASTEXITCODE
}
finally { Pop-Location }
