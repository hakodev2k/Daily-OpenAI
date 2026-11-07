$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/AccessCodeLab.csproj -- --run
    if ($LASTEXITCODE -ne 0) { throw "run failed (exit code $LASTEXITCODE)." }
} finally { Pop-Location }
