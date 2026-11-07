$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'verify.ps1')
if (-not $?) { throw 'Run failed' }
