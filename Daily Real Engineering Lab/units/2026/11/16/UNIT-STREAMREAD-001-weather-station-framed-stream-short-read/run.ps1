$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try { dotnet run --project WeatherStreamLab.csproj --; $result = $LASTEXITCODE } finally { Pop-Location }
if ($result -ne 0) { exit $result }
