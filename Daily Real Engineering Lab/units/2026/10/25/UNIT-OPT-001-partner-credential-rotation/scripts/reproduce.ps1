$ErrorActionPreference = 'Stop'
$unit = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $unit
try {
    & dotnet run --project './starter/RotationLab.csproj' -- reproduce
    if ($LASTEXITCODE -ne 0) { throw "Original incident was not reproduced (exit $LASTEXITCODE)." }
} finally {
    Pop-Location
}
