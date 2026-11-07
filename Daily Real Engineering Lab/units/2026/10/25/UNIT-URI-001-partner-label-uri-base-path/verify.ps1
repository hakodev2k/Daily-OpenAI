$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/RouteLab.csproj -- --verify
    if ($LASTEXITCODE -ne 0) { throw 'Learner starter did not satisfy route contract' }
} finally { Pop-Location }
