$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project './starter/ClockLab.csproj' --configuration Release -- reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Baseline symptom not reproduced; run on original starter.' }
} finally { Pop-Location }
