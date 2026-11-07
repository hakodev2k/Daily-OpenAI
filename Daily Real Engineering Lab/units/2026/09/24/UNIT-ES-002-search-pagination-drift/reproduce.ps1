$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/SearchPaginationLab.csproj | Out-String
Write-Host $output
if ($output -notmatch 'duplicates=10') { throw 'Expected pagination drift was not reproduced.' }
Write-Host 'Reproduced: an item appears in both pages after the result set changes.'