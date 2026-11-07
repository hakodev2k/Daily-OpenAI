$output = & "$PSScriptRoot/scripts/validate.ps1" -Deployment "$PSScriptRoot/starter/deployment.yaml" 2>&1
$output | ForEach-Object { Write-Host $_ }
if (($output -join "`n") -notmatch 'READY_CONTRACT_BROKEN') { throw 'Không reproduce được trạng thái starter mong đợi.' }
Write-Host 'REPRODUCED'
