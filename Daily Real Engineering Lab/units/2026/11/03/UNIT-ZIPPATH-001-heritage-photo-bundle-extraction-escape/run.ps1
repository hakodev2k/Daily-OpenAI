$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/ArchiveImportLab.csproj --configuration Release -- reproduce
    if ($LASTEXITCODE -ne 0) { throw "reproduce failed (exit $LASTEXITCODE)." }
}
finally { Pop-Location }
