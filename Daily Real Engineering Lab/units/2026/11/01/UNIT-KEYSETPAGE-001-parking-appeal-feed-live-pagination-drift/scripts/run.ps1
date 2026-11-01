$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root "starter/AppealFeed.csproj")
if ($LASTEXITCODE -ne 0) { throw "Starter run failed ($LASTEXITCODE)." }
