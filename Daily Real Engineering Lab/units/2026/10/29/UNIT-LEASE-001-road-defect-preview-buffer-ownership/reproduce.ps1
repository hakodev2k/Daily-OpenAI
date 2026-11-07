$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/PreviewLeaseLab.csproj -- reproduce
    $code = $LASTEXITCODE
}
finally {
    Pop-Location
}
exit $code
