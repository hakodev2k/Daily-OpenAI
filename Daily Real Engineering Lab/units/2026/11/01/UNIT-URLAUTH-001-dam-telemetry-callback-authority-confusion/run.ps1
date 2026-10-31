$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet run --project './starter/CallbackGate.csproj' --configuration Release
    $code = $LASTEXITCODE
} finally { Pop-Location }
exit $code
