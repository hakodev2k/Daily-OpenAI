$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore ./starter/ReturnXmlLab.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    dotnet build ./starter/ReturnXmlLab.csproj --no-restore --configuration Release
    exit $LASTEXITCODE
}
finally { Pop-Location }
