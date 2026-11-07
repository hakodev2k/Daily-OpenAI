$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore starter/EncodingLab.Starter.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    dotnet restore solution/EncodingLab.Reference.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally { Pop-Location }
