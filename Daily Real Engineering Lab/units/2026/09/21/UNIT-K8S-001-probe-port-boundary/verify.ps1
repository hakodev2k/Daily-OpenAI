$output = & "$PSScriptRoot/scripts/validate.ps1" -Deployment "$PSScriptRoot/starter/deployment.yaml" 2>&1
$output | ForEach-Object { Write-Host $_ }
if (($output -join "`n") -notmatch 'READY_CONTRACT_OK') { throw 'FAIL: readiness contract vẫn chưa hợp lệ.' }
if ((Get-Content -Raw "$PSScriptRoot/starter/deployment.yaml") -notmatch 'ASPNETCORE_URLS[\s\S]*?8080') { throw 'FAIL: không được né bài bằng cách đổi application listener.' }
Write-Host 'PASS'
