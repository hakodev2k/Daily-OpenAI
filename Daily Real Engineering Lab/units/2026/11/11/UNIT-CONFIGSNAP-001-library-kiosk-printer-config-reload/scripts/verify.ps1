$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/Kiosk.csproj'
$output = @(dotnet run --project $project)
if ($LASTEXITCODE -ne 0) { throw 'Starter failed to run' }
$output | ForEach-Object { Write-Output $_ }
$expected = @('first=KIOSK-A:OLD:BOOK-104','second=KIOSK-B:NEW:BOOK-105','registryVersion=2','activePrefix=NEW','refreshObserved=True')
foreach ($line in $expected) { if ($output -notcontains $line) { throw "VERIFY FAILED: missing $line" } }
Write-Output 'VERIFY PASSED: both before and after configuration contracts hold'

