$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/HarborFrame.csproj'
dotnet run --project $project -- fragmented
if ($LASTEXITCODE -ne 0) { Write-Host 'PASS: starter symptom reproduced'; exit 0 }
Write-Error 'FAIL: starter did not reproduce expected symptom'; exit 1
