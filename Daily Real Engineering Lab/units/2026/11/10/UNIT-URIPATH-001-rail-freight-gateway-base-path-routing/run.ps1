$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try { dotnet run --project starter/RailGateway.csproj; if ($LASTEXITCODE -ne 0) { throw "Run failed" } } finally { Pop-Location }
