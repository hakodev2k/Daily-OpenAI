$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/VesselArchive.csproj -- demo
    $exitCode = $LASTEXITCODE
} finally { Pop-Location }
exit $exitCode
