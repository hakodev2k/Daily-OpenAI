$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root "starter/LibraryGrantApproval.csproj") -- --reproduce
if ($LASTEXITCODE -ne 0) { throw "Original symptom was not reproduced ($LASTEXITCODE). Use the original starter state." }
Write-Host "Reproduction confirmed: the sample was accepted with an unexpected posted amount."
