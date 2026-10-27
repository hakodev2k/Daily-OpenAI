$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/PartnerProbe.csproj -- reproduce
    $code = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $code
