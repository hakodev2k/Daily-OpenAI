$ErrorActionPreference = 'Stop'
$work = Join-Path $PSScriptRoot '.lab-work'
if (-not (Test-Path -LiteralPath (Join-Path $work '.git'))) { throw 'Run ./setup.ps1 first' }
# New untracked file not present in the initial reproduction: negative regression.
'{"policyId":"UNEXPECTED-NEW","enabled":true}' | Set-Content -LiteralPath (Join-Path $work 'release/unexpected-new.json') -Encoding UTF8
& (Join-Path $PSScriptRoot 'run.ps1')
& (Join-Path $PSScriptRoot 'checks/Assert-Release.ps1') -Mode fixed
