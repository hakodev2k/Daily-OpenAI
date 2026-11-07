$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..')
try {
    & dotnet run --project './starter/ArrivalGateway.csproj' -- inspect
    exit $LASTEXITCODE
}
finally { Pop-Location }
