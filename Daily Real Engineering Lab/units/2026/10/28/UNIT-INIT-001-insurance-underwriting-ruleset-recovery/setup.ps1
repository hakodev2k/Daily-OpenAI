$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore ./starter/RulesetLab.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Starter restore failed' }
    dotnet restore ./solution/RulesetLab.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Reference restore failed' }
    Write-Host 'SETUP_OK'
}
finally { Pop-Location }
