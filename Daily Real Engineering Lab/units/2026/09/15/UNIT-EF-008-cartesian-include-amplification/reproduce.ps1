$ErrorActionPreference='Stop'
$out = dotnet run --project ./starter/Lab.csproj
$out | Write-Host
$rows = [int](($out | Where-Object { $_ -like 'RowsProcessed=*' }) -replace 'RowsProcessed=','')
if ($rows -le 100) { throw 'Expected amplified starter work was not reproduced.' }
Write-Host 'REPRODUCED: business result is correct while intermediate work is amplified.'
