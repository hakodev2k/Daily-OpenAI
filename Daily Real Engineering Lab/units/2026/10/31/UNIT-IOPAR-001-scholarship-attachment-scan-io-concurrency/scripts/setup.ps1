$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    & dotnet --version
    if ($LASTEXITCODE -ne 0) { exit 1 }
    & dotnet restore './starter/AttachmentScanLab.csproj'
    if ($LASTEXITCODE -ne 0) { exit 1 }
    & dotnet restore './solution/AttachmentScanLab.csproj'
    if ($LASTEXITCODE -ne 0) { exit 1 }
    Write-Output 'SETUP_OK'
}
finally { Pop-Location }
