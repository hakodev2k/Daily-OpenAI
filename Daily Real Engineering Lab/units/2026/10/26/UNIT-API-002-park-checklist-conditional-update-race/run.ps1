param([ValidateSet("reproduce", "verify")][string]$Mode = "reproduce")
$ErrorActionPreference = "Stop"
& dotnet run --project (Join-Path $PSScriptRoot "starter/ChecklistLab.csproj") -- $Mode
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
