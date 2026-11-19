$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try { dotnet run --project BaggageGateway.csproj -- --verify; if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE } } finally { Pop-Location }
