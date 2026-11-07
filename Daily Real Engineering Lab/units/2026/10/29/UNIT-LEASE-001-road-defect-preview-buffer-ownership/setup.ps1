$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore ./starter/PreviewLeaseLab.csproj
    $code = $LASTEXITCODE
}
finally {
    Pop-Location
}
exit $code
