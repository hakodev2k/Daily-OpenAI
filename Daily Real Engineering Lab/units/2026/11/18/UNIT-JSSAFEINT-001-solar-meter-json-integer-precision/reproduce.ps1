$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/MeterReplay.csproj'
$output = & dotnet run --project $project
if ($LASTEXITCODE -ne 0) { throw "Starter failed to run" }
$output | ForEach-Object { Write-Host $_ }
if (-not ($output -match '^DISTINCT_CLIENT_IDS=1$') -or -not ($output -match '^ID_ROUNDTRIP_OK=false$')) { throw "Expected failure was not reproduced" }
Write-Host 'REPRODUCED: two distinct IDs collapse at the client boundary.'

