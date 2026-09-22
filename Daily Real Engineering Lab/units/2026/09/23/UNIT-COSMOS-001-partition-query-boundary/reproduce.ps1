$ErrorActionPreference = 'Stop'
$out = dotnet run --project ./starter/Starter.csproj
$out | Write-Output
$text = $out -join "`n"
if ($text -notmatch 'records=20' -or $text -notmatch 'partitionsTouched=8') { throw 'Expected starter symptom was not reproduced.' }
Write-Host 'Reproduced: result is correct while query work spans all logical partitions.'