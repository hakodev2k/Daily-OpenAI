param([ValidateSet("demo", "reproduce", "verify")][string]$Mode = "demo")
$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/AuditLab.Starter.csproj"
dotnet run --project $project -- $Mode
exit $LASTEXITCODE
