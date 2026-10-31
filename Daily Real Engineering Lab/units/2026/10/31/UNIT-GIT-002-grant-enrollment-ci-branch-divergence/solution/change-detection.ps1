param(
    [Parameter(Mandatory = $true)][string]$RepoPath,
    [string]$BaseRef = 'main',
    [string]$HeadRef = 'feature'
)
$ErrorActionPreference = 'Stop'
$mergeBases = @(& git -C $RepoPath merge-base $BaseRef $HeadRef)
if ($LASTEXITCODE -ne 0 -or $mergeBases.Count -ne 1) {
    throw "Không xác định được một tổ tiên chung duy nhất của $BaseRef và $HeadRef."
}
$changedFiles = & git -C $RepoPath diff --name-only --no-renames $mergeBases[0] $HeadRef
if ($LASTEXITCODE -ne 0) {
    throw "Không thể đọc thay đổi của $HeadRef kể từ điểm phân nhánh."
}
$changedFiles | ForEach-Object { $_.Trim() } | Where-Object { $_.Length -gt 0 }
