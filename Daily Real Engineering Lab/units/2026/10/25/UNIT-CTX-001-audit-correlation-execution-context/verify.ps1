$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/CorrelationLab.csproj -- --verify
    if ($LASTEXITCODE -ne 0) { throw 'Learner starter failed audit correlation contract' }
} finally { Pop-Location }
