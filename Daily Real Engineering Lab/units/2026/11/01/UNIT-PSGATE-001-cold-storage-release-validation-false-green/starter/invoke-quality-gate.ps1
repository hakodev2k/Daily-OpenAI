param(
    [Parameter(Mandatory = $false)]
    [ValidateSet('pass', 'fail')]
    [string]$Mode = 'pass'
)
$ErrorActionPreference = 'Stop'
# Giữ hành vi nhất quán giữa các PowerShell agent cũ và mới.
$PSNativeCommandUseErrorActionPreference = $false

$validator = Join-Path $PSScriptRoot '..\fixtures\validator.cmd'
Write-Output "QUALITY_GATE_START: $Mode"
& $validator $Mode
Write-Output "QUALITY_GATE_PASS: $Mode"
