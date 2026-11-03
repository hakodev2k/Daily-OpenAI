param([string]$Project = "starter/WebhookLab.csproj")
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project (Join-Path $root $Project)
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
