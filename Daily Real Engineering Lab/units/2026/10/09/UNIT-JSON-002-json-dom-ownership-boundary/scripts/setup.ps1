$ErrorActionPreference = 'Stop'
foreach ($name in @('baseline', 'starter', 'solution')) {
    & dotnet restore (Join-Path $PSScriptRoot "../$name/JsonLab.csproj")
    if ($LASTEXITCODE -ne 0) { throw "Restore failed: $name" }
}
Write-Output 'SETUP_PASSED'
