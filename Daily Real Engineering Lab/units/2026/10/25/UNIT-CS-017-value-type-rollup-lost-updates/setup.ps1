$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore starter/FieldOpsLab.Starter.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    dotnet restore solution/FieldOpsLab.Reference.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally { Pop-Location }
