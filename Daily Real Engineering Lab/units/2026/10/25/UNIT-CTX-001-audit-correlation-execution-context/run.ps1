$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/CorrelationLab.csproj -- --inspect
    if ($LASTEXITCODE -ne 0) { throw 'Inspection run failed' }
} finally { Pop-Location }
