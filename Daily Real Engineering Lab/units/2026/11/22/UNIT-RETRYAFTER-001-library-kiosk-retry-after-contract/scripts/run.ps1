$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/KioskBackoff.csproj')
exit $LASTEXITCODE
