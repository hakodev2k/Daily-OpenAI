$ErrorActionPreference='Stop'
$p=Join-Path $PSScriptRoot 'starter/ArchiveTags.csproj'
dotnet build $p --nologo | Out-Host
if($LASTEXITCODE -ne 0){throw 'Build failed'}
$o= & dotnet run --no-build --project $p --no-launch-profile 2>&1 | Out-String
$c=$LASTEXITCODE; Write-Host $o
if($c -ne 2 -or $o -notmatch 'TIMEOUT tag.length=4001'){throw "Unexpected signature exit=$c"}
Write-Host 'PASS: original failure reproduced.'
