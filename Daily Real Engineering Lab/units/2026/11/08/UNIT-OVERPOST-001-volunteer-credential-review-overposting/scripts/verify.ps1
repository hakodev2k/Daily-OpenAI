$ErrorActionPreference='Stop'
$out = & dotnet run --project (Join-Path $PSScriptRoot '../starter/VolunteerLab.csproj') 2>&1
$code = $LASTEXITCODE
$out | ForEach-Object { Write-Output $_ }
if ($code -eq 0 -and ($out -join "`n") -match 'ordinary=Lan Nguyen/Pending' -and ($out -join "`n") -match 'adversarial=Lan Nguyen/Pending' -and ($out -join "`n") -match 'VERIFY PASS') { exit 0 }
Write-Error 'Contract verification failed'; exit 1
