$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet --version
    if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK is unavailable' }
    & dotnet restore './probe/ScannerLab.Probe.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Starter restore failed' }
    & dotnet restore './solution/ScannerLab.Reference.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Reference restore failed' }
    Write-Host 'SETUP_OK'
}
finally { Pop-Location }
