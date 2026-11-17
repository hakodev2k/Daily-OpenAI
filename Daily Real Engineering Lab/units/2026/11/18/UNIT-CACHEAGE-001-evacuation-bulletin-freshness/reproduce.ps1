$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try { dotnet run --project ./starter/Lab.csproj -- reproduce; if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE } } finally { Pop-Location }
