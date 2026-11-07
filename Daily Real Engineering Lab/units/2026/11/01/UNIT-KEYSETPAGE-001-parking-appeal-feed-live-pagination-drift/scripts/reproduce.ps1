$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root "starter/AppealFeed.csproj") -- --reproduce
if ($LASTEXITCODE -ne 0) { throw "Original symptom not reproduced ($LASTEXITCODE). Use the original starter." }
Write-Host "Reproduction confirmed: overlap between two pages after a concurrent insert."
