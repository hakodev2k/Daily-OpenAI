$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet run --project ./starter/PatrolLab.csproj -- verify
    if ($LASTEXITCODE -ne 0) { throw 'Learner verification failed' }
}
finally { Pop-Location }
