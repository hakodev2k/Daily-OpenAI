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
if ($code -eq 0) {
    Write-Host 'VERIFIED: all tenant-isolation regression assertions passed.'
    exit 0
}
Write-Host "NOT_VERIFIED: learner-editable starter returned exit code $code."
exit 1
