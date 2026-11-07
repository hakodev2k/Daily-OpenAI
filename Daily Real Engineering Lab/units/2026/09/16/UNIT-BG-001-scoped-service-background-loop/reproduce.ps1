$ErrorActionPreference='Stop'
$out = dotnet run --project ./starter/WorkerLab.csproj
$out | Write-Host
if ($out -notmatch 'cycle=2; database=v2; observed=v1') { throw 'Không reproduce được triệu chứng mong đợi.' }
Write-Host 'Reproduced: cycle 2 vẫn quan sát state của cycle trước.'