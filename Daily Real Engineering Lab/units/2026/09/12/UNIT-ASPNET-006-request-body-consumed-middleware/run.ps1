$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/WebhookLab.csproj'
$env:ASPNETCORE_URLS = 'http://127.0.0.1:5066'
dotnet run --project $project
