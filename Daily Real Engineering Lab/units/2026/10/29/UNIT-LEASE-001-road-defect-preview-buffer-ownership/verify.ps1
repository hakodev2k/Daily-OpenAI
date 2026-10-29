$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/PreviewLeaseLab.csproj -- verify
    $code = $LASTEXITCODE
}
finally {
    Pop-Location
}
exit $code
