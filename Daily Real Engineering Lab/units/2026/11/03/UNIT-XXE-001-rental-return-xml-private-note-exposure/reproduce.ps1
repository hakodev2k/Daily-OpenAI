$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    # Pass means the original controlled security symptom was reproduced.
    dotnet run --project ./starter/ReturnXmlLab.csproj --configuration Release -- --reproduce
    exit $LASTEXITCODE
}
finally { Pop-Location }
