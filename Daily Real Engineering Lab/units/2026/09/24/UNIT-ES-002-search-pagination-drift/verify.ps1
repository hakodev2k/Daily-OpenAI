$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/SearchPaginationLab.csproj | Out-String
Write-Host $output
if ($output -notmatch 'mutation=ticket-15-updated') { throw 'Verification requires the concurrent update scenario to remain active.' }
if ($output -match 'duplicates=\d') { throw 'A ticket is still duplicated across the pagination session.' }
if ($output -notmatch 'uniqueCount=20') { throw 'The first two pages do not contain 20 unique tickets.' }
Write-Host 'PASS: learner-editable starter preserves the mutation while returning stable page membership.'