$ErrorActionPreference='Stop'
$p=Join-Path $PSScriptRoot 'starter/ArchiveTags.csproj'
dotnet build $p --nologo | Out-Host
if($LASTEXITCODE -ne 0){throw 'Build failed'}
$o= & dotnet run --no-build --project $p --no-launch-profile 2>&1 | Out-String
$c=$LASTEXITCODE; Write-Host $o
if($c -ne 0 -or $o -notmatch 'tag.length=4001 valid=False' -or $o -notmatch 'tag.length=7 valid=True' -or $o -notmatch 'tag.length=8 valid=True'){throw "FAIL: validation contract exit=$c"}
Write-Host 'PASS: learner-workspace contract.'
