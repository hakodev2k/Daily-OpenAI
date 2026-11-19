$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try { dotnet run --project BaggageGateway.csproj -- --reproduce; if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE } } finally { Pop-Location }
