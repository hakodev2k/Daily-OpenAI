$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    & dotnet run --project 'starter/Starter.csproj' -- demo
    $exitCode = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $exitCode
