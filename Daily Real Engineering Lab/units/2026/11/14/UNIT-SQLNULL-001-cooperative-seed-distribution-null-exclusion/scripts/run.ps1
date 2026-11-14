$ErrorActionPreference = 'Stop'
$base = Split-Path -Parent $PSScriptRoot
python (Join-Path $base 'starter/check.py') run
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
