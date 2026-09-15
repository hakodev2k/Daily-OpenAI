$ErrorActionPreference = 'Stop'

$unitRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = (git -C $unitRoot rev-parse --show-toplevel).Trim()
$relativePath = (Resolve-Path "$unitRoot/starter/build.sh").Path.Substring($repoRoot.Length + 1).Replace('\','/')

Write-Host "== Local content execution =="
if (Get-Command bash -ErrorAction SilentlyContinue) {
    bash "$unitRoot/starter/build.sh"
} else {
    Write-Host "bash not found; skipping content execution. Git metadata check remains deterministic."
}

Write-Host "`n== CI checkout metadata preflight =="
$stage = git -C $repoRoot ls-files --stage -- $relativePath
if (-not $stage) { throw "starter/build.sh is not tracked by Git." }

$mode = ($stage -split '\s+')[0]
Write-Host "Git index mode: $mode"

if ($mode -eq '100755') {
    throw "Reproduction did not fail: the script is already tracked as executable. Reset the lab before retrying."
}

Write-Host "EXPECTED FAILURE: Linux CI expects direct execution, but Git will check out this script without executable mode."
exit 0
