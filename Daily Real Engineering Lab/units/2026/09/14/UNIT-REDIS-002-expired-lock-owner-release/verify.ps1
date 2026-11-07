$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
$output = & dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) { throw "Expected learner-fixed starter to exit 0; got $LASTEXITCODE." }
if (($output -join "`n") -notmatch 'SAFE') { throw 'Expected SAFE marker was not produced.' }
if (($output -join "`n") -match 'VIOLATION') { throw 'Ownership violation is still present.' }
Write-Host 'VERIFIED: stale cleanup cannot remove a newer owner lease.'
