$ErrorActionPreference = 'Stop'
if (-not (Get-Command git -ErrorAction SilentlyContinue)) { throw 'Git CLI not found' }
$work = Join-Path $PSScriptRoot '.lab-work'
if (Test-Path -LiteralPath $work) { Remove-Item -LiteralPath $work -Recurse -Force }
New-Item -ItemType Directory -Path (Join-Path $work 'release') -Force | Out-Null

& git -C $work init -q
if ($LASTEXITCODE -ne 0) { throw 'git init failed' }
& git -C $work config user.name 'Engineering Lab'
& git -C $work config user.email 'lab@example.invalid'
& git -C $work config core.autocrlf false

'{"policyId":"POOL-OLD","enabled":true}' | Set-Content -LiteralPath (Join-Path $work 'release/retired.json') -Encoding UTF8
& git -C $work add -A
& git -C $work commit -q -m 'Historical safety policy'
if ($LASTEXITCODE -ne 0) { throw 'Historical commit failed' }

Remove-Item -LiteralPath (Join-Path $work 'release/retired.json') -Force
'{"policyId":"POOL-2026","enabled":true}' | Set-Content -LiteralPath (Join-Path $work 'release/active.json') -Encoding UTF8
& git -C $work add -A
& git -C $work commit -q -m 'Approve new safety policy and retire old policy'
if ($LASTEXITCODE -ne 0) { throw 'Release commit failed' }

# Simulate leftovers restored by a prior runner job after checkout.
'{"policyId":"POOL-OLD","enabled":true}' | Set-Content -LiteralPath (Join-Path $work 'release/retired.json') -Encoding UTF8
'{"policyId":"DEBUG-ONLY","enabled":true}' | Set-Content -LiteralPath (Join-Path $work 'release/partner-debug.json') -Encoding UTF8

Write-Host 'Synthetic CI workspace ready:'
& git -C $work status --short
Write-Host 'Files tracked in the approved Git revision:'
& git -C $work ls-files -- 'release/*.json'
