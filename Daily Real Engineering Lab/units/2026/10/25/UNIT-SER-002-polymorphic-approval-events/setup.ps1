$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore starter/Lab.Starter.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    dotnet restore solution/Lab.Reference.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally { Pop-Location }
