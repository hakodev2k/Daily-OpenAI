param([ValidateSet("reproduce","verify")][string]$Mode="reproduce")
$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/ShiftSyncLab.csproj"
& dotnet run --project $project -- $Mode
exit $LASTEXITCODE
