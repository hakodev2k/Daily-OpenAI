$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/CorrelationLab.csproj -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Expected starter symptom was not reproduced' }
} finally { Pop-Location }
