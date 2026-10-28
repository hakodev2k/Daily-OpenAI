$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$code = -1
Push-Location $root
try {
    dotnet run --project ./starter/Starter.csproj --configuration Release
    $code = $LASTEXITCODE
}
finally {
    Pop-Location
}
if ($code -eq 17) {
    Write-Host 'REPRODUCTION_CONFIRMED: starter violates tenant isolation.'
    exit 0
}
if ($code -eq 0) {
    Write-Host 'REPRODUCTION_NOT_FOUND: starter already satisfies the isolation contract.'
    exit 1
}
Write-Host "REPRODUCTION_ERROR: build/runtime failed with exit code $code."
exit 1
