param(
    [ValidateSet('reproduce', 'verify')]
    [string]$Mode = 'reproduce'
)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $root
try {
    dotnet run --project ./starter/AlertDispatchLab.csproj -- $Mode
    $result = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $result
