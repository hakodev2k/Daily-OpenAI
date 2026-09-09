$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'reproduce.ps1')

Write-Host ''
Write-Host 'Reproduction complete. Record hypotheses, edit starter/, then run:'
Write-Host '  ./verify.ps1'
