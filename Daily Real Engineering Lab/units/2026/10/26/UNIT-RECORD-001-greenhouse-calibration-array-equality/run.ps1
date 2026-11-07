param(
    [ValidateSet("run", "reproduce", "verify")]
    [string]$Mode = "run"
)
$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "harness/GreenhouseLab.Harness.csproj"
dotnet run --project $project --configuration Release -- $Mode
exit $LASTEXITCODE
