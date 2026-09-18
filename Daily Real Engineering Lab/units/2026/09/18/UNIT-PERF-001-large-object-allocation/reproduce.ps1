$ErrorActionPreference='Stop'
$out = dotnet run --project ./starter/PerfLab.csproj -- 40
$out | Write-Host
$peak = [long](($out | Where-Object { $_ -like 'peakLiveBytes=*' }) -replace 'peakLiveBytes=','')
if ($peak -lt 5000000) { throw 'Expected high peak live bytes was not reproduced.' }
Write-Host 'Expected memory-pressure symptom reproduced.'