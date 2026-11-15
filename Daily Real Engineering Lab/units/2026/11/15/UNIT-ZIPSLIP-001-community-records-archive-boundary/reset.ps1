$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot 'sandbox'
if (Test-Path $root) { Remove-Item $root -Recurse -Force }
Write-Host 'Sandbox reset'
