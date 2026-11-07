$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/Starter.csproj"
$output | Write-Host
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
if ($output -notmatch 'preloadedLines=3') { throw 'Preload behavior regressed.' }
if ($output -notmatch 'openInStore=2') { throw 'Seeded business data changed unexpectedly.' }
if ($output -notmatch 'returnedLines=2') { throw 'Expected learner fix to return exactly 2 open lines.' }
if ($output -match 'statuses=.*Closed') { throw 'Closed line is still present in returned graph.' }
Write-Host 'VERIFIED: learner-edited starter returns only open lines without removing preload behavior.'
