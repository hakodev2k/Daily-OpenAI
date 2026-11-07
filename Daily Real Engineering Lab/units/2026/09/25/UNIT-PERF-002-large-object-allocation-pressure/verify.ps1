$ErrorActionPreference='Stop'
$out=dotnet run --project ./starter/PerfLab.csproj -c Release
$out
$line=$out | Where-Object { $_ -like 'allocated=*' }
if(-not $line){throw 'Missing allocation evidence.'}
$allocated=[int64]($line -replace 'allocated=','')
if($allocated -gt 12000000){throw "Allocation remains too high: $allocated"}
Write-Host 'Verification passed.'