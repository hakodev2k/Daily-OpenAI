$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $root
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    dotnet restore ./starter/AlertDispatchLab.csproj
    $result = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $result
