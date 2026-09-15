$ErrorActionPreference='Stop'
$out = dotnet run --project "$PSScriptRoot/starter/CdnVariantLab.csproj"
$out | ForEach-Object { Write-Host $_ }
if (@($out | Where-Object { $_ -match 'match=False' }).Count -ne 0) { throw 'Verification failed: at least one request received the wrong representation.' }
$hits = @($out | Where-Object { $_ -match 'EDGE-HIT' }).Count
if ($hits -lt 2) { throw 'Verification failed: correctness was achieved by effectively disabling cache reuse.' }
Write-Host 'PASS: locale correctness preserved and safe edge-cache reuse remains observable.'
