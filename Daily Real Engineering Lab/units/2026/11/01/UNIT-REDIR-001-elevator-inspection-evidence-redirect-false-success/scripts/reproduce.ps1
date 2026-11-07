$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..')
try {
    & dotnet run --project './starter/EvidenceHandoff.csproj' -- reproduce
    exit $LASTEXITCODE
}
finally { Pop-Location }
