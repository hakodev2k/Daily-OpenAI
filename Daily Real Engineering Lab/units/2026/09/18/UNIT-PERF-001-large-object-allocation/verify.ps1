$ErrorActionPreference='Stop'
$out = dotnet run --project ./starter/PerfLab.csproj -- 40
$out | Write-Host
$peak = [long](($out | Where-Object { $_ -like 'peakLiveBytes=*' }) -replace 'peakLiveBytes=','')
$count = [int](($out | Where-Object { $_ -like 'outputCount=*' }) -replace 'outputCount=','')
if ($count -ne 40) { throw 'Functional regression: output count changed.' }
if ($peak -gt 1048576) { throw 'Memory property still fails: peak live bytes remain too high.' }
Write-Host 'VERIFY PASSED'