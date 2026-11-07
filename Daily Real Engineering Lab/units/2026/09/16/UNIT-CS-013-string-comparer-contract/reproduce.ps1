$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/StringComparerLab.csproj
$output | ForEach-Object { Write-Host $_ }
if ($output -notmatch 'sku-alpha\|found=False' -or $output -notmatch 'Sku-Beta\|found=False') { throw 'Starter did not reproduce the expected false lookup misses.' }
Write-Host 'Reproduced: business-equivalent identifiers can miss lookup.'