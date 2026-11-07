$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    & dotnet run --project './starter/AttachmentScanLab.csproj' -c Release -- --check
    $code = $LASTEXITCODE
}
finally { Pop-Location }
exit $code
