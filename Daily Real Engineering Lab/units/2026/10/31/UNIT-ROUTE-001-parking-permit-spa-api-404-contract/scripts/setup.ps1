$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    & dotnet --version
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    & dotnet restore "./contract/Contract.csproj"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    & dotnet build "./contract/Contract.csproj" -c Release --no-restore
    exit $LASTEXITCODE
}
finally {
    Pop-Location
}
