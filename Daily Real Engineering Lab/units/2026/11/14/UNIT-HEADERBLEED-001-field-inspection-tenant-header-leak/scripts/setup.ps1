$ErrorActionPreference = 'Stop'
dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build (Join-Path $PSScriptRoot '../starter/FieldInspection.csproj')
exit $LASTEXITCODE
