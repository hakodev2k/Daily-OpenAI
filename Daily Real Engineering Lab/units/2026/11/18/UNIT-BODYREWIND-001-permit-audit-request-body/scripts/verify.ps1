$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/PermitAuditLab.csproj'
dotnet run --project $project -- --verify
exit $LASTEXITCODE
