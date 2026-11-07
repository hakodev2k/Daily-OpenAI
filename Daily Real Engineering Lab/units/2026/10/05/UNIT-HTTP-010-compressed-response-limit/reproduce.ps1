$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet build "$root/starter/Client/Client.csproj"
if ($LASTEXITCODE -ne 0) { throw "Starter build failed." }
dotnet run --no-build --project "$root/starter/Client/Client.csproj"
if ($LASTEXITCODE -ne 0) { throw "Starter run failed." }
Write-Host "REPRODUCE=PASS when output shows materialized payload exceeds configured limit."
