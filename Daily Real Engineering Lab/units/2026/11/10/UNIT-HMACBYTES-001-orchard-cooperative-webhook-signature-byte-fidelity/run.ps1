$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/WebhookLab.csproj" --configuration Release -- --mode verify
if ($LASTEXITCODE -ne 0) { throw "Starter contract failed" }
