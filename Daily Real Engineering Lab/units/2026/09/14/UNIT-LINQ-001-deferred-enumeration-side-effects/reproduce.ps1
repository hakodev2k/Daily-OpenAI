$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/Lab.csproj
$output | Write-Host
$calls = [int](($output | Where-Object { $_ -like 'CatalogCalls=*' }) -replace 'CatalogCalls=', '')
if ($calls -le 4) { throw "Expected dependency-call amplification in starter, got $calls calls." }
Write-Host "Reproduced: catalog was called $calls times for 4 SKUs."
