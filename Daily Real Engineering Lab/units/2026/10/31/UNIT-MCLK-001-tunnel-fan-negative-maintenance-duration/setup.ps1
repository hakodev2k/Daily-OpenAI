$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore './starter/ClockLab.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
    dotnet build './starter/ClockLab.csproj' --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
} finally { Pop-Location }
