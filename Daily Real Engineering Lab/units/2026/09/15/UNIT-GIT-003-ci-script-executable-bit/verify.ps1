$ErrorActionPreference = 'Stop'

$unitRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = (git -C $unitRoot rev-parse --show-toplevel).Trim()
$relativePath = (Resolve-Path "$unitRoot/starter/build.sh").Path.Substring($repoRoot.Length + 1).Replace('\','/')

$stage = git -C $repoRoot ls-files --stage -- $relativePath
if (-not $stage) { throw "starter/build.sh is not tracked by Git." }

$mode = ($stage -split '\s+')[0]
Write-Host "Git index mode: $mode"

if ($mode -ne '100755') {
    throw "VERIFY FAILED: Git index still does not persist executable mode for starter/build.sh."
}

$diff = git -C $repoRoot diff --cached -- $relativePath
Write-Host "VERIFY PASSED: executable mode is persisted in the Git index."
Write-Host "Review the staged diff before committing:"
Write-Host $diff
