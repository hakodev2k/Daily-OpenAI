$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet restore ./starter/PipelineLab.csproj
    $result = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $result
