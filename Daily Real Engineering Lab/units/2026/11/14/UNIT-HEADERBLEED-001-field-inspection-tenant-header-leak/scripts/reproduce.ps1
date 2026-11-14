$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/FieldInspection.csproj') -- reproduce
exit $LASTEXITCODE
