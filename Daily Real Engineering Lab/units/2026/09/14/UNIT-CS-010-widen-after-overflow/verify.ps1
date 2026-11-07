$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try {
    dotnet run --configuration Release -- --assert-fixed
    if ($LASTEXITCODE -ne 0) { exit 1 }
    Write-Host 'Verification passed.'
}
finally {
    Pop-Location
}
