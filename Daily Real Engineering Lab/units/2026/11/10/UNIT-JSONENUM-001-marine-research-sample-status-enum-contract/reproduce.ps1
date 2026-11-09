& (Join-Path $PSScriptRoot 'run.ps1')
if ($LASTEXITCODE -eq 0) { throw 'Starter unexpectedly passes all contract cases' }
Write-Host 'Reproduced contract violation'
exit 0
