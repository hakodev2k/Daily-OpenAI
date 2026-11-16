$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try { dotnet run --project WeatherStreamLab.csproj -- --verify; $result = $LASTEXITCODE } finally { Pop-Location }
if ($result -ne 0) { throw "VERIFY failed on starter code" }
Write-Host "VERIFY PASS"
