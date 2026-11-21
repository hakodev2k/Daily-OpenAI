$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  foreach ($culture in @('en-US','fr-FR')) {
    Write-Host "=== $culture ==="
    dotnet run --project ./starter/WeatherImport.csproj -- $culture
    if ($LASTEXITCODE -ne 0) { throw "dotnet run failed: $culture" }
  }
} finally { Pop-Location }
