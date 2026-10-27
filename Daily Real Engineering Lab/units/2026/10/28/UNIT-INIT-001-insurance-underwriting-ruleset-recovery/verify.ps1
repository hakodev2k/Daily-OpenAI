$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/RulesetLab.csproj -- verify
    if ($LASTEXITCODE -ne 0) { throw 'Learner workspace verification failed' }
}
finally { Pop-Location }
