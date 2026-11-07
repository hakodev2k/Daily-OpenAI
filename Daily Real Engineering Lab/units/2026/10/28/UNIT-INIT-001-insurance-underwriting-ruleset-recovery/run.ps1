$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/RulesetLab.csproj -- run
    if ($LASTEXITCODE -ne 0) { throw 'Starter run failed' }
}
finally { Pop-Location }
