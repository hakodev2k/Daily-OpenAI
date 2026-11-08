$ErrorActionPreference='Stop'
$out = & dotnet run --project (Join-Path $PSScriptRoot '../starter/VolunteerLab.csproj') 2>&1
$code = $LASTEXITCODE
$out | ForEach-Object { Write-Output $_ }
if ($code -eq 1 -and ($out -join "`n") -match 'adversarial=Lan Nguyen/Approved' -and ($out -join "`n") -match 'VERIFY FAIL') { Write-Output 'REPRODUCED'; exit 0 }
Write-Error 'Starter symptom not reproduced'; exit 1
