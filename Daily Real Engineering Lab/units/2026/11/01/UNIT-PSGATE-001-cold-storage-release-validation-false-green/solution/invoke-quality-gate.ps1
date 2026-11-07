param(
    [Parameter(Mandatory = $false)]
    [ValidateSet('pass', 'fail')]
    [string]$Mode = 'pass'
)
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false

$validator = Join-Path $PSScriptRoot '..\fixtures\validator.cmd'
Write-Output "QUALITY_GATE_START: $Mode"
& $validator $Mode
$validatorExitCode = $LASTEXITCODE

if ($validatorExitCode -ne 0) {
    Write-Output "QUALITY_GATE_FAILED: validator exit code $validatorExitCode"
    exit $validatorExitCode
}
Write-Output "QUALITY_GATE_PASS: $Mode"
exit 0
