$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { throw 'Missing .NET SDK' }
    dotnet build ./starter/VesselArchive.csproj --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Starter build failed' }
    dotnet build ./solution/VesselArchive.csproj --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Reference build failed' }
} finally { Pop-Location }
