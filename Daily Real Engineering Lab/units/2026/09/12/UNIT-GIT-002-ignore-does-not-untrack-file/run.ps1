$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'reproduce.ps1')
exit $LASTEXITCODE
