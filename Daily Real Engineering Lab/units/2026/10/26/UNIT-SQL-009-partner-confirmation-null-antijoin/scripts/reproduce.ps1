$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..'))
function Invoke-Docker([string[]] $ArgsList) {
  & docker @ArgsList
  if ($LASTEXITCODE -ne 0) { throw "docker exited with code $LASTEXITCODE" }
}
Invoke-Docker -ArgsList @('compose','-p','unit-sql-009','up','-d','--wait')
Invoke-Docker -ArgsList @('compose','-p','unit-sql-009','exec','-T','db','psql','-X','-U','lab','-d','lab','-v','ON_ERROR_STOP=1','-f','/lab/scripts/reset.sql')
Invoke-Docker -ArgsList @('compose','-p','unit-sql-009','exec','-T','db','psql','-X','-U','lab','-d','lab','-v','ON_ERROR_STOP=1','-f','/lab/scripts/original-query.sql')
Invoke-Docker -ArgsList @('compose','-p','unit-sql-009','exec','-T','db','psql','-X','-U','lab','-d','lab','-v','ON_ERROR_STOP=1','-f','/lab/scripts/assert-before.sql')
Write-Host 'Baseline reproduced. Inspect evidence, then edit starter/query.sql.'
