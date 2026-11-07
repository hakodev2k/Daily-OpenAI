param(
    [Parameter(Mandatory = $true)][string]$RepoPath,
    [string]$BaseRef = 'main',
    [string]$HeadRef = 'feature'
)
$ErrorActionPreference = 'Stop'

# Script CI chọn file cần build/test theo Pull Request.
$changedFiles = & git -C $RepoPath diff --name-only --no-renames $BaseRef $HeadRef
if ($LASTEXITCODE -ne 0) {
    throw "Không thể đọc diff của $BaseRef và $HeadRef."
}
$changedFiles | ForEach-Object { $_.Trim() } | Where-Object { $_.Length -gt 0 }
