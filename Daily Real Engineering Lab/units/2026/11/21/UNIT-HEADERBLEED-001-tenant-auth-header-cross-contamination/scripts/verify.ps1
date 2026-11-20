$unit = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $unit 'starter/TenantDispatch.csproj') -- verify
exit $LASTEXITCODE
