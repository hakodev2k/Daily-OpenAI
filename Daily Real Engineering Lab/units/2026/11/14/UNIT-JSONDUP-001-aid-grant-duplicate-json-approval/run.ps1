$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try { dotnet run --project ./starter/GrantReplay.csproj -- } finally { Pop-Location }
