$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    & dotnet run --project 'baseline/Baseline.csproj' -- reproduce
    $exitCode = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $exitCode
