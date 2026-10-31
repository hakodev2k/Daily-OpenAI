$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet run --project './starter/PartnerRetryLab.csproj' -- --reproduce
    $result = $LASTEXITCODE
}
finally { Pop-Location }
exit $result
