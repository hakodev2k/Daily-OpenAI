$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'replay.ps1') -Verify
if ($LASTEXITCODE -ne 0) { exit 1 }
