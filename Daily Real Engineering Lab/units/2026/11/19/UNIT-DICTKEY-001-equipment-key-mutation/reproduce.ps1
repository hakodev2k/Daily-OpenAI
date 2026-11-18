$ErrorActionPreference = 'Stop'
$out = dotnet run --project (Join-Path $PSScriptRoot 'starter/Registry.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Starter build/run failed' }
$out | ForEach-Object { Write-Output $_ }
if (($out -join "`n") -notmatch 'before=True count=1' -or ($out -join "`n") -notmatch 'after=False count=1' -or ($out -join "`n") -notmatch 'remove=False count=1') { throw 'Expected failure was not reproduced' }
Write-Output 'REPRODUCED: registry loses lookup after tag update'
