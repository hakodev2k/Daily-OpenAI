$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project './starter/ClockLab.csproj' --configuration Release -- verify
    if ($LASTEXITCODE -ne 0) { throw 'Verification failed on learner-edited starter.' }
} finally { Pop-Location }
