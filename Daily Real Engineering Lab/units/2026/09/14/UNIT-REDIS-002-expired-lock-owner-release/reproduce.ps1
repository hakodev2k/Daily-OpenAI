$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
$output = & dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 2) { throw "Expected starter to reproduce the ownership violation with exit code 2; got $LASTEXITCODE." }
if (($output -join "`n") -notmatch 'VIOLATION') { throw 'Expected VIOLATION marker was not produced.' }
Write-Host 'REPRODUCED: stale owner cleanup removed a newer owner lease.'
