$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    dotnet restore ./starter/AlertQueue.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally { Pop-Location }
