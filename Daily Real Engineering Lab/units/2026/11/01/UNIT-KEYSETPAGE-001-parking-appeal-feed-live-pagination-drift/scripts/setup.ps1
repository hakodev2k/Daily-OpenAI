$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet --version
if ($LASTEXITCODE -ne 0) { throw ".NET SDK is unavailable." }
dotnet build (Join-Path $root "starter/AppealFeed.csproj") --nologo
if ($LASTEXITCODE -ne 0) { throw "Starter build failed." }
dotnet build (Join-Path $root "checks/ContractChecks.csproj") --nologo
if ($LASTEXITCODE -ne 0) { throw "Contract build failed." }
Write-Host "Setup complete."
