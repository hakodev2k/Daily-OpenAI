$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/HarborInventory.csproj'
dotnet run --project $project
if ($LASTEXITCODE -eq 0) { Write-Error 'No failure reproduced'; exit 1 }
Write-Host 'REPRODUCED: starter contract failed'
exit 0
