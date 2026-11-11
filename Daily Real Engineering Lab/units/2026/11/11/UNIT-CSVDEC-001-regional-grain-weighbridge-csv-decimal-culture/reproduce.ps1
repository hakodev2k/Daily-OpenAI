$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Weighbridge.csproj'
$failures = 0
foreach ($culture in @('en-US','de-DE','vi-VN')) {
  dotnet run --project $project -- $culture
  if ($LASTEXITCODE -ne 0) { $failures++ }
}
if ($failures -eq 0) { throw 'Reproduction failed: starter unexpectedly satisfies every culture contract.' }
Write-Host "REPRODUCED: $failures culture(s) violate the import contract."
