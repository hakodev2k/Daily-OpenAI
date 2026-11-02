$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    & dotnet run --project (Join-Path $root 'baseline/Baseline.csproj') -- reproduce
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally { Pop-Location }
