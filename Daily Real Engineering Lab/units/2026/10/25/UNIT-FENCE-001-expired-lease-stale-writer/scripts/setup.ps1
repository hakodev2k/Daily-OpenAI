$ErrorActionPreference = 'Stop'
$unitRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $unitRoot
try {
    & dotnet restore 'harness/Harness.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed' }
    & dotnet build 'harness/Harness.csproj' --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }
} finally {
    Pop-Location
}
