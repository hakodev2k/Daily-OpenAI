$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { throw ".NET SDK unavailable" }
    dotnet restore "./starter/CoastalAlertLab.csproj"
    $code = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $code
