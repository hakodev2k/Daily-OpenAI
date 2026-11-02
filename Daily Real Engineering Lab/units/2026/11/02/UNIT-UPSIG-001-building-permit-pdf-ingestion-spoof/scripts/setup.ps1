$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    & dotnet --version
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    & dotnet restore (Join-Path $root 'baseline/Baseline.csproj')
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    & dotnet restore (Join-Path $root 'harness/Harness.csproj')
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Write-Host 'SETUP OK'
} finally { Pop-Location }
