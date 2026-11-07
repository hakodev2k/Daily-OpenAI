$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    & dotnet restore "./contract/Contract.csproj"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    & dotnet build "./contract/Contract.csproj" -c Release --no-restore
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    & dotnet build "./solution/Solution.csproj" -c Release
    exit $LASTEXITCODE
}
finally { Pop-Location }
