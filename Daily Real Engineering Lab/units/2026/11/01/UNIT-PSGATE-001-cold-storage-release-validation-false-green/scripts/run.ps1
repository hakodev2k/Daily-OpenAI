$ErrorActionPreference = 'Stop'
$starter = Join-Path $PSScriptRoot '..\starter\invoke-quality-gate.ps1'
& $starter -Mode pass
