$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/RouteLab.csproj -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Original starter symptom was not reproduced' }
} finally { Pop-Location }
