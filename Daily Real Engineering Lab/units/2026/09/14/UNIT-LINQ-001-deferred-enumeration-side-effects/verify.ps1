$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/Lab.csproj
$output | Write-Host
$total = ($output | Where-Object { $_ -like 'Total=*' }) -replace 'Total=', ''
$expensive = ($output | Where-Object { $_ -like 'Expensive=*' }) -replace 'Expensive=', ''
$calls = [int](($output | Where-Object { $_ -like 'CatalogCalls=*' }) -replace 'CatalogCalls=', '')
if ($total -ne '100') { throw "Functional regression: expected Total=100, got $total." }
if ($expensive -ne '2') { throw "Functional regression: expected Expensive=2, got $expensive." }
if ($calls -ne 4) { throw "Expected exactly 4 catalog calls, got $calls." }
Write-Host 'Verification passed.'
