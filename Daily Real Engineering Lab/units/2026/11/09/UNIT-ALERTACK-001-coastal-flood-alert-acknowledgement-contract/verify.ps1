$ErrorActionPreference = 'Stop'
$decision = Get-Content (Join-Path $PSScriptRoot 'workspace/decision.md') -Raw
$required = @('State machine','Expiry','Reorder','Relay failure','Offline','Trade-offs')
$missing = @($required | Where-Object { $decision -notmatch [regex]::Escape($_) })
if ($missing.Count -gt 0) { throw "Missing decision headings: $($missing -join ', ')" }
if ($decision -match 'TODO|Điền vào đây') { throw 'Complete decision template before verification' }
Write-Output 'Documentation completeness PASS. Manually evaluate every scenario in docs/acceptance.md; this script cannot certify physical safety.'
