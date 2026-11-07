$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    & dotnet run --project "./contract/Contract.csproj" -c Release -- reproduce
    exit $LASTEXITCODE
}
finally { Pop-Location }
