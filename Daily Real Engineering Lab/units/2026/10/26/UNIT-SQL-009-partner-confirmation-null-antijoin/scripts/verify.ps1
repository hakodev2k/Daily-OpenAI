$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..'))
function Invoke-Docker([string[]] $ArgsList) {
  & docker @ArgsList
  if ($LASTEXITCODE -ne 0) { throw "docker exited with code $LASTEXITCODE" }
}
Invoke-Docker @('compose','-p','unit-sql-009','up','-d','--wait')
Invoke-Docker @('compose','-p','unit-sql-009','exec','-T','db','psql','-X','-U','lab','-d','lab','-v','ON_ERROR_STOP=1','-f','/lab/scripts/reset.sql')
Invoke-Docker @('compose','-p','unit-sql-009','exec','-T','db','psql','-X','-U','lab','-d','lab','-v','ON_ERROR_STOP=1','-f','/lab/starter/query.sql')
Invoke-Docker @('compose','-p','unit-sql-009','exec','-T','db','psql','-X','-U','lab','-d','lab','-v','ON_ERROR_STOP=1','-f','/lab/scripts/assert-after.sql')
Write-Host 'Learner query passed exact-set and isolation assertions.'
