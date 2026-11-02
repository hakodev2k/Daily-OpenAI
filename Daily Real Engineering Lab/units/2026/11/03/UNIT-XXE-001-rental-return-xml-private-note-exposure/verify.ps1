$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    # Tests learner-edited starter, not solution/.
    dotnet run --project ./starter/ReturnXmlLab.csproj --configuration Release -- --verify
    exit $LASTEXITCODE
}
finally { Pop-Location }
