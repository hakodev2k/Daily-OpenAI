param(
    [Parameter(Mandatory=$true)][string] $WorkingCopy,
    [Parameter(Mandatory=$true)][string] $BaseSha
)
$ErrorActionPreference = 'Continue'
if ($BaseSha -notmatch '^[0-9a-fA-F]{40}$') {
    Write-Output 'CHANGE_GATE_ERROR: invalid base SHA format'
    exit 20
}
& git -C $WorkingCopy rev-parse --is-inside-work-tree 1>$null 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Output 'CHANGE_GATE_ERROR: invalid checkout'
    exit 21
}
# The local fixture's main branch points exactly at the supplied comparison base.
# Fetch that commit's tree without fetching the entire repository history.
& git -C $WorkingCopy fetch --quiet --no-tags --depth=1 origin main 1>$null 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Output 'CHANGE_GATE_ERROR: cannot fetch comparison branch'
    exit 22
}
& git -C $WorkingCopy cat-file -e ($BaseSha + '^{commit}') 1>$null 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Output 'CHANGE_GATE_ERROR: supplied base commit is unavailable'
    exit 23
}
$changed = @(& git -C $WorkingCopy diff --name-only $BaseSha HEAD -- 2>$null)
if ($LASTEXITCODE -ne 0) {
    Write-Output 'CHANGE_GATE_ERROR: diff failed'
    exit 24
}
$changed | ForEach-Object { Write-Output $_ }
exit 0
