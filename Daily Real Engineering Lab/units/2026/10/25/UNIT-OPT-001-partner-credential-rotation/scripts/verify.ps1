$ErrorActionPreference = 'Stop'
$unit = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $unit
try {
    & dotnet run --project './starter/RotationLab.csproj' -- verify
    if ($LASTEXITCODE -ne 0) { throw "Learner starter verification failed (exit $LASTEXITCODE)." }
} finally {
    Pop-Location
}
