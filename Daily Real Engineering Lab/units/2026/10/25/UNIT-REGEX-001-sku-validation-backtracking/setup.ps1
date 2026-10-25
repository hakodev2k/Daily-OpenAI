$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    dotnet restore ./starter/RegexLab.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    dotnet restore ./solution/RegexLab.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Write-Host 'Setup completed.'
} finally { Pop-Location }
