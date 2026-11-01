$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..')
try {
    & dotnet run --project './starter/ArrivalGateway.csproj' -- reproduce
    exit $LASTEXITCODE
}
finally { Pop-Location }
