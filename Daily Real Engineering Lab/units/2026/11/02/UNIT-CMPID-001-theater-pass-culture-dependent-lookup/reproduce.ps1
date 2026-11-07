$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/AccessCodeLab.csproj -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw "Original symptom was not reproduced (exit code $LASTEXITCODE)." }
} finally { Pop-Location }
