$ErrorActionPreference = 'Stop'
$unitRoot = Split-Path -Parent $PSScriptRoot
$starter = Join-Path $unitRoot 'starter\invoke-quality-gate.ps1'
$engineName = if ($PSVersionTable.PSEdition -eq 'Core') { 'pwsh.exe' } else { 'powershell.exe' }
$engine = Join-Path $PSHOME $engineName
if (-not (Test-Path -LiteralPath $engine)) { throw "Không tìm thấy PowerShell engine: $engine" }

$failureOutput = & $engine -NoProfile -NonInteractive -ExecutionPolicy Bypass -File $starter -Mode fail 2>&1
$failureExit = $LASTEXITCODE
$successOutput = & $engine -NoProfile -NonInteractive -ExecutionPolicy Bypass -File $starter -Mode pass 2>&1
$successExit = $LASTEXITCODE
$failureText = $failureOutput -join "`n"
$successText = $successOutput -join "`n"
Write-Output "OBSERVE: fail exit=$failureExit; pass exit=$successExit"
Write-Output "FAIL CASE OUTPUT:"
Write-Output $failureText
if ($failureExit -eq 0 -and $successExit -eq 0 -and
    $failureText.Contains('VALIDATOR_FAIL') -and
    $failureText.Contains('QUALITY_GATE_PASS') -and
    $successText.Contains('VALIDATOR_PASS')) {
    Write-Output 'REPRODUCED: validator failed but release gate reported success.'
    exit 0
}
Write-Output 'NOT_REPRODUCED: starter symptom not observed; inspect output or environment.'
exit 1
