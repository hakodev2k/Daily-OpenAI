$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet restore ./starter/PatrolLab.csproj
    if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed' }
    dotnet build ./starter/PatrolLab.csproj --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }
}
finally { Pop-Location }
