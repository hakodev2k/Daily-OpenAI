$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/ExportProbe.csproj" -- --check
$code = $LASTEXITCODE
if ($code -ne 2) { throw "Reproduction failed: expected starter check exit code 2, got $code" }
Write-Host "REPRODUCED: starter violates byte-integrity contract."
