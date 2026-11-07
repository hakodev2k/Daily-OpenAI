$ErrorActionPreference='Stop'
$out = dotnet run --project ./starter/Lab.csproj
$out | Write-Host
$rows = [int](($out | Where-Object { $_ -like 'RowsProcessed=*' }) -replace 'RowsProcessed=','')
$result = $out | Where-Object { $_ -like 'BusinessResult=*' }
if ($result -ne 'BusinessResult=Lines:40;Adjustments:25') { throw 'Business result regressed.' }
if ($rows -gt 66) { throw "Work amplification remains too high: RowsProcessed=$rows" }
Write-Host 'VERIFIED: business result preserved and intermediate work amplification removed.'
