$ErrorActionPreference = 'Stop'
$base = Split-Path -Parent $PSScriptRoot
python (Join-Path $base 'starter/check.py') reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
