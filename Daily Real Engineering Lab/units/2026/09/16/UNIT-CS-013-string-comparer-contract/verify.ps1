$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/StringComparerLab.csproj
$output | ForEach-Object { Write-Host $_ }
$required = @('SKU-ALPHA|found=True|product=Product-101','sku-alpha|found=True|product=Product-101','Sku-Beta|found=True|product=Product-202','SKU-GAMMA|found=False|product=-')
foreach ($line in $required) { if ($output -notcontains $line) { throw "Verification failed: missing expected behavior: $line" } }
Write-Host 'PASS: lookup follows the stated identifier contract without matching an unrelated identifier.'