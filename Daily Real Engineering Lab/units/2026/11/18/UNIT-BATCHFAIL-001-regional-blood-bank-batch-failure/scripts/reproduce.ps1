$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot '..'
$out = @(dotnet run --project (Join-Path $root 'starter/BatchAudit.csproj') 2>&1)
if ($LASTEXITCODE -ne 0) { $out | ForEach-Object { Write-Output $_ }; exit 2 }
$out | ForEach-Object { Write-Output $_ }
if (($out -join "`n") -match 'accepted=0; rejected=1') { Write-Output 'REPRODUCED: batch report does not account for every input'; exit 0 }
Write-Error 'Starter symptom not reproduced'; exit 1
