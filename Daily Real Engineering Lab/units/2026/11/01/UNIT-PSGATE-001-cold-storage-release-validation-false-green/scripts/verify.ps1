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

$checks = @(
    @{ Name = 'Validator thật sự phát hiện lỗi'; Passed = $failureText.Contains('VALIDATOR_FAIL') },
    @{ Name = 'Wrapper báo lỗi cho tiến trình cha'; Passed = ($failureExit -ne 0) },
    @{ Name = 'Không in QUALITY_GATE_PASS khi validator lỗi'; Passed = (-not $failureText.Contains('QUALITY_GATE_PASS')) },
    @{ Name = 'Validator pass được quan sát'; Passed = $successText.Contains('VALIDATOR_PASS') },
    @{ Name = 'Wrapper pass có exit code 0'; Passed = ($successExit -eq 0) },
    @{ Name = 'Wrapper pass giữ thông báo thành công'; Passed = $successText.Contains('QUALITY_GATE_PASS') }
)
foreach ($check in $checks) {
    $status = if ($check.Passed) { 'PASS' } else { 'FAIL' }
    Write-Output "$status : $($check.Name)"
}
if ($checks | Where-Object { -not $_.Passed }) {
    Write-Output "VERIFY_FAILED: fail exit=$failureExit, pass exit=$successExit"
    exit 1
}
Write-Output 'VERIFY_PASSED: both failure and success paths satisfy release-gate contract.'
exit 0
