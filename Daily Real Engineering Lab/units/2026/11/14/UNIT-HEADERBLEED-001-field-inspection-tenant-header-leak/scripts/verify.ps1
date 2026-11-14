$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/FieldInspection.csproj') -- verify
exit $LASTEXITCODE
