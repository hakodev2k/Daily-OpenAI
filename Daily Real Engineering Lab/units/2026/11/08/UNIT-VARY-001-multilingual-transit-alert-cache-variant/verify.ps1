$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try { dotnet run --project starter/TransitLab.csproj -- verify; if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE } } finally { Pop-Location }
