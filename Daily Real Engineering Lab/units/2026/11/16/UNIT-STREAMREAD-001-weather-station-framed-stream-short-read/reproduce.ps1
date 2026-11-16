$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try { dotnet run --project WeatherStreamLab.csproj -- --reproduce; $result = $LASTEXITCODE } finally { Pop-Location }
if ($result -ne 0) { throw "REPRODUCE failed on starter code" }
Write-Host "REPRODUCE PASS"
