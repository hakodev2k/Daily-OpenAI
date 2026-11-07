$ErrorActionPreference = "Stop"
Push-Location (Join-Path $PSScriptRoot "starter")
try {
    dotnet run --configuration Release -- --reproduce
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally { Pop-Location }
