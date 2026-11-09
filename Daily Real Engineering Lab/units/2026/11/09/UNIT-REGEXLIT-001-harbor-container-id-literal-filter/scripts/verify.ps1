$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/HarborInventory.csproj'
dotnet run --project $project
if ($LASTEXITCODE -ne 0) { Write-Error 'Verification failed'; exit 1 }
Write-Host 'VERIFIED'
exit 0
