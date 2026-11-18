$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot '..'
$out = @(dotnet run --project (Join-Path $root 'starter/BatchAudit.csproj') 2>&1)
if ($LASTEXITCODE -ne 0) { $out | ForEach-Object { Write-Output $_ }; exit 2 }
$out | ForEach-Object { Write-Output $_ }
$text=$out -join "`n"
$required=@('accepted=2; rejected=2','ACCEPT A-101','ACCEPT D-404','REJECT B-202','REJECT C-303')
foreach($item in $required) { if(-not $text.Contains($item)) { Write-Error "Missing expected result: $item"; exit 1 } }
if (($out | Where-Object { $_ -match '^(ACCEPT|REJECT) ' }).Count -ne 4) { Write-Error 'Expected exactly four per-record outcomes'; exit 1 }
Write-Output 'PASS: starter reports four independent outcomes'
