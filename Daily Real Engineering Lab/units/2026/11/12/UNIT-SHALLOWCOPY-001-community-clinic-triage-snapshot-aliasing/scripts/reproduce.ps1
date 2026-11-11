$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$output=@(dotnet run --project (Join-Path $root 'starter/Triage.csproj'))
if($LASTEXITCODE -ne 0){throw 'Starter failed'}
$output | ForEach-Object { Write-Host $_ }
$before=($output | Where-Object {$_ -like 'before=*'} | Select-Object -First 1)
$after=($output | Where-Object {$_ -like 'after=*'} | Select-Object -First 1)
if($before -eq 'before=urgent,routine' -and $after -eq 'after=routine,routine'){Write-Host 'REPRODUCED: saved snapshot drift';exit 0}
Write-Host 'No original symptom; starter may be modified'
