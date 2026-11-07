$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/VesselArchive.csproj -- reproduce
    $exitCode = $LASTEXITCODE
} finally { Pop-Location }
exit $exitCode
