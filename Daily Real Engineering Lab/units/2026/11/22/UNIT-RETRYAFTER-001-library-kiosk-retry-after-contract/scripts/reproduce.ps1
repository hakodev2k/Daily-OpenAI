$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/KioskBackoff.csproj') -- --reproduce
exit $LASTEXITCODE
