$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/AuditLab.Starter.csproj"
dotnet restore $project
exit $LASTEXITCODE
