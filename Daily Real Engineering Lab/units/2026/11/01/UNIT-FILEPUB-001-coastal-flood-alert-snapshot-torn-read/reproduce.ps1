$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet run --project "./starter/CoastalAlertLab.csproj" --configuration Release -- reproduce
    $code = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $code
