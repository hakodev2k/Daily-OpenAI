$ErrorActionPreference = 'Stop'
$work = Join-Path $PSScriptRoot '.lab-work'
if (-not (Test-Path -LiteralPath (Join-Path $work '.git'))) { throw 'Run ./setup.ps1 first' }
$out = Join-Path $work 'artifact'
& (Join-Path $PSScriptRoot 'starter/Package.ps1') -Workspace $work -Output $out
if (-not (Test-Path -LiteralPath (Join-Path $out 'manifest.json'))) { throw 'Missing release manifest' }
Write-Host 'Release manifest:'
Get-Content -LiteralPath (Join-Path $out 'manifest.json')
