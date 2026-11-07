$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/Starter.csproj"
$output | Write-Host
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
if ($output -notmatch 'openInStore=2') { throw 'Expected seeded store to contain 2 open lines.' }
if ($output -notmatch 'returnedLines=3') { throw 'Expected starter symptom: 3 returned lines.' }
Write-Host 'REPRODUCED: filtered read returns an out-of-filter line.'
