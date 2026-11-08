$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/WaterQuota.csproj'
$output = & dotnet run --project $project -- --verify 2>&1
$output | ForEach-Object { Write-Output $_ }
if ($LASTEXITCODE -eq 0) { throw 'Expected starter to violate tenant isolation; reproduction did not fail.' }
if (-not (($output | Out-String) -match 'isolation=False')) { throw 'Unexpected failure: isolation evidence missing.' }
Write-Output 'REPRODUCED: tenant isolation check failed as expected.'
