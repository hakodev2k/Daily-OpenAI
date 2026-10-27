$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'setup.ps1') | Out-Null
$work = Join-Path $PSScriptRoot '.lab-runtime/shallow'
$baseSha = (Get-Content (Join-Path $PSScriptRoot '.lab-runtime/base.sha') -Raw).Trim()
$learner = Join-Path $PSScriptRoot 'starter/detect-changes.ps1'
$output = @(& $learner -WorkingCopy $work -BaseSha $baseSha)
$code = $LASTEXITCODE
if ($code -ne 0) {
    Write-Output "FAIL: change gate exited $code; output=$($output -join ' ')"
    exit 1
}
$actual = @($output | ForEach-Object { "$_".Trim() } | Where-Object { $_ -ne '' } | Sort-Object)
$expected = @('docs/permit-rules.md', 'src/PermitEngine/Rules.cs')
if ($actual.Count -ne $expected.Count -or ($actual -join '|') -ne ($expected -join '|')) {
    Write-Output "FAIL: expected $($expected -join ', '), got $($actual -join ', ')"
    exit 1
}
$invalid = '0000000000000000000000000000000000000000'
$ignored = @(& $learner -WorkingCopy $work -BaseSha $invalid)
$invalidExit = $LASTEXITCODE
if ($invalidExit -eq 0) {
    Write-Output 'FAIL: nonexistent base SHA was accepted.'
    exit 1
}
Write-Output 'PASS: exact changed files and fail-closed invalid base handling.'
exit 0
