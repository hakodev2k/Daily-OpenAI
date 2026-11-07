$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { throw "dotnet SDK not available." }
    dotnet restore ./starter/AccessCodeLab.csproj
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed." }
    dotnet build ./starter/AccessCodeLab.csproj --no-restore
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed." }
} finally { Pop-Location }
