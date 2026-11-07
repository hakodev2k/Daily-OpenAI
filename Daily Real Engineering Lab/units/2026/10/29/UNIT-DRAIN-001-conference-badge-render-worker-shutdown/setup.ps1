$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { throw 'Missing .NET SDK 8.x' }
    dotnet restore './starter/BadgeDrainLab.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Restore failed' }
} finally { Pop-Location }
