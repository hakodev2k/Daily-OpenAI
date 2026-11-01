$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/AccessCodeLab.csproj -- --verify
    if ($LASTEXITCODE -ne 0) { throw "Learner code failed the identity contract (exit code $LASTEXITCODE)." }
} finally { Pop-Location }
