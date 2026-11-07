$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..')
try {
    & dotnet run --project './starter/EvidenceHandoff.csproj' -- verify
    exit $LASTEXITCODE
}
finally { Pop-Location }
