$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet run --project ./starter/PipelineLab.csproj -- verify
    $result = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $result
