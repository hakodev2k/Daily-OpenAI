$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PartnerWebhookLab.csproj'
dotnet run --project $project -- run
exit $LASTEXITCODE
