$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root "starter/LibraryGrantApproval.csproj")
if ($LASTEXITCODE -ne 0) { throw "starter run failed ($LASTEXITCODE)" }
