$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$adr = Get-Content (Join-Path $root 'workspace/decision.md') -Raw
$required = @('## Decision context','## Evidence','## Hypotheses','## Options','## Decision & protocol','## Failure injection matrix','## Rollout, rollback, metrics','## Residual risks')
foreach ($section in $required) { if (-not $adr.Contains($section)) { Write-Error "Missing ADR section: $section"; exit 1 } }
if ($adr.Contains('(Điền') -or $adr.Contains('(Trích') -or $adr.Contains('(Mô tả')) { Write-Error 'ADR still contains starter placeholders'; exit 1 }
Write-Host 'ADR structural check passed. Manual rubric review still required.'
