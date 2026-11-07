$ErrorActionPreference = 'Stop'
$unit = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $unit
try {
    & dotnet run --project './starter/RotationLab.csproj' -- demo
    if ($LASTEXITCODE -ne 0) { throw "demo failed with exit code $LASTEXITCODE" }
} finally {
    Pop-Location
}
