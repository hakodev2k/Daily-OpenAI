$unit = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $unit 'starter/TenantDispatch.csproj') -- reproduce
exit $LASTEXITCODE
