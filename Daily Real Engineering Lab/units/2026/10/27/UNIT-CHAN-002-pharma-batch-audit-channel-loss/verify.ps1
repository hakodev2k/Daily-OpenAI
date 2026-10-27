$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/AuditLab.Starter.csproj"
dotnet run --project $project -- verify
exit $LASTEXITCODE
