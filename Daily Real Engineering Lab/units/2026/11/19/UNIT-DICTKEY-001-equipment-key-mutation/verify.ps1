$ErrorActionPreference = 'Stop'
$out = dotnet run --project (Join-Path $PSScriptRoot 'starter/Registry.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Build/run failed' }
$out | ForEach-Object { Write-Output $_ }
if (($out -join "`n") -notmatch 'before=True count=1' -or ($out -join "`n") -notmatch 'after=True count=1' -or ($out -join "`n") -notmatch 'remove=True count=0') { throw 'REGRESSION: mutable key still breaks registry contract' }
Write-Output 'PASS: lookup and removal remain consistent after update'
