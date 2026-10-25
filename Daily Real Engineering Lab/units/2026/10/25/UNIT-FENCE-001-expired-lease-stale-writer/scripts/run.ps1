$ErrorActionPreference = 'Stop'
$unitRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $unitRoot
try {
    & dotnet run --project 'harness/Harness.csproj' -- --demo
    if ($LASTEXITCODE -ne 0) { throw 'demo failed with exit code ' + $LASTEXITCODE }
} finally {
    Pop-Location
}
