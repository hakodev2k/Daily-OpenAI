& (Join-Path $PSScriptRoot "run.ps1") -Mode reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
