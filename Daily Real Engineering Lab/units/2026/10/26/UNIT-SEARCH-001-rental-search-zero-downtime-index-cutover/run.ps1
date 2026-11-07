$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'starter/capacity-model.ps1')
if (-not $?) { throw 'Capacity model failed' }
