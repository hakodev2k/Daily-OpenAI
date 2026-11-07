param(
    [ValidateSet("demo", "reproduce", "verify")]
    [string]$Mode = "demo"
)
$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/ColdChainReceiptLab.csproj"
& dotnet run --project $project --configuration Release -- $Mode
exit $LASTEXITCODE
