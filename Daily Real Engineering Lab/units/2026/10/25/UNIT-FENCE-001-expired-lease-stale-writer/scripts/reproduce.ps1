$ErrorActionPreference = 'Stop'
$unitRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $unitRoot
try {
    & dotnet run --project 'harness/Harness.csproj' -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw 'reproduce failed with exit code ' + $LASTEXITCODE }
} finally {
    Pop-Location
}
