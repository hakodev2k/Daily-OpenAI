$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/ReturnXmlLab.csproj --configuration Release -- --reproduce
    exit $LASTEXITCODE
}
finally { Pop-Location }
