$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet restore ./starter/StreamUploadLab.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    dotnet build ./starter/StreamUploadLab.csproj --no-restore
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    dotnet run --project ./starter/StreamUploadLab.csproj --no-build -- reproduce
    exit $LASTEXITCODE
}
finally {
    Pop-Location
}
