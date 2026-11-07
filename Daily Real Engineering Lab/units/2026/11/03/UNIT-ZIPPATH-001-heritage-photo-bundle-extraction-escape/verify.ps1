$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/ArchiveImportLab.csproj --configuration Release -- verify
    if ($LASTEXITCODE -ne 0) { throw "verify failed (exit $LASTEXITCODE)." }
}
finally { Pop-Location }
