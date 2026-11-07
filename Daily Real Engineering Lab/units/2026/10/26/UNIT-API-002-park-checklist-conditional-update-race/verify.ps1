& (Join-Path $PSScriptRoot "run.ps1") -Mode verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
