$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/LinqOrderingLab.csproj"
$output | ForEach-Object { Write-Host $_ }
$joined = ($output -join "`n")
if ($joined -notmatch 'COUNT=3') { throw 'Reproduction failed: expected COUNT=3.' }
if ($joined -notmatch 'ORDER=INV-C,INV-B,INV-A') { throw 'Reproduction failed: intended ordering symptom was not observed.' }
Write-Host 'REPRODUCED: business priority ordering is incorrect while all invoices are present.'
