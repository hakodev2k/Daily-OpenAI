$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet restore "./starter/PartnerCookieLab.csproj"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally { Pop-Location }
