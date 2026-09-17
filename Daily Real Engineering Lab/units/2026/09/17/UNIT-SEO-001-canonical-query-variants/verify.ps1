$ErrorActionPreference = 'Stop'
$out = dotnet run --project ./starter/SeoCanonicalLab.csproj
$out
$canonicalLines = @($out | Where-Object { $_ -like 'Canonical:*' })
$trackingLines = @($canonicalLines | Select-Object -First 3)
foreach ($line in $trackingLines) {
  if ($line -ne 'Canonical: https://shop.local/products/42') { throw "Tracking/presentation dimension still changes canonical identity: $line" }
}
$variantLines = @($canonicalLines | Select-Object -Skip 3)
foreach ($line in $variantLines) {
  if ($line -notmatch 'variant=blue') { throw "Resource variant was collapsed unexpectedly: $line" }
  if ($line -match 'utm_') { throw "Tracking dimension leaked into canonical identity: $line" }
}
Write-Host 'PASS: canonical identity is stable for equivalent requests and preserves resource-defining variant.'