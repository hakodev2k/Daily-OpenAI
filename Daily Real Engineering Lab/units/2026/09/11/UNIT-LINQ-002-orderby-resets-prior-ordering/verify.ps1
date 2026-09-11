$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/LinqOrderingLab.csproj"
$output | ForEach-Object { Write-Host $_ }
$joined = ($output -join "`n")
if ($joined -notmatch 'COUNT=3') { throw 'Verification failed: expected all 3 invoices.' }
if ($joined -notmatch 'ORDER=INV-C,INV-A,INV-B') { throw 'Verification failed: required Priority then DueDate ordering was not preserved.' }
Write-Host 'VERIFIED: ordering contract is satisfied.'
