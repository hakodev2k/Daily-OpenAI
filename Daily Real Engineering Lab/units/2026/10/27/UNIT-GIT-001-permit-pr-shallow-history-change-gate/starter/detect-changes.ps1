param(
    [Parameter(Mandatory=$true)][string] $WorkingCopy,
    [Parameter(Mandatory=$true)][string] $BaseSha
)
$ErrorActionPreference = 'Continue'
$changed = @(& git -C $WorkingCopy diff HEAD^ HEAD --name-only 2>$null)
if ($LASTEXITCODE -ne 0) {
    Write-Output 'CHANGE_GATE_ERROR: cannot determine changed files'
    exit 12
}
$changed | ForEach-Object { Write-Output $_ }
exit 0
