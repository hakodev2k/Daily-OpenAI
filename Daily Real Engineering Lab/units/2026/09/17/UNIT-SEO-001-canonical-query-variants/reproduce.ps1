$ErrorActionPreference = 'Stop'
$out = dotnet run --project ./starter/SeoCanonicalLab.csproj
$out
if ($out -notmatch 'Canonical: https://shop.local/products/42\?utm_source=newsletter') { throw 'Starter symptom was not reproduced.' }
Write-Host 'Reproduced: equivalent product requests publish distinct canonical identities.'