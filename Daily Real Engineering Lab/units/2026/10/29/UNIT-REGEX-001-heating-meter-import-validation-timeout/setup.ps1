$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK not found' }
    dotnet restore './starter/MeterCodeLab.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'restore failed' }
} finally { Pop-Location }
