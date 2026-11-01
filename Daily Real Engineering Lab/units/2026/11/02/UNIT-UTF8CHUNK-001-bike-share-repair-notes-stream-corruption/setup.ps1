$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK is unavailable.' }
    dotnet restore ./starter/Utf8ChunkLab.csproj
    if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed.' }
    dotnet build ./starter/Utf8ChunkLab.csproj --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed.' }
} finally { Pop-Location }
