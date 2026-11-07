$ErrorActionPreference = 'Stop'
$attributes = Get-Content (Join-Path $PSScriptRoot 'starter/.gitattributes') -Raw
if ($attributes -notmatch '(?m)^\*\s+text=auto\s+eol=lf\s*$') {
  throw 'VERIFY_FAIL: repository text normalization policy is not explicit yet.'
}
$root = Join-Path $PSScriptRoot '.verify-work'
Remove-Item $root -Recurse -Force -ErrorAction SilentlyContinue
New-Item $root -ItemType Directory | Out-Null
Push-Location $root
try {
  git init -q
  git config user.email 'lab@example.invalid'
  git config user.name 'Engineering Lab'
  git config core.autocrlf false
  Copy-Item ../starter/.gitattributes ./.gitattributes
  Copy-Item ../starter/appsettings.sample ./appsettings.sample
  git add .
  git commit -q -m baseline
  $text = (Get-Content ./appsettings.sample -Raw) -replace 'TimeoutSeconds=30','TimeoutSeconds=45'
  $text = $text -replace "`r?`n", "`r`n"
  [IO.File]::WriteAllText((Join-Path $PWD 'appsettings.sample'), $text)
  git add --renormalize appsettings.sample
  $num = git diff --cached --numstat -- appsettings.sample
  Write-Host "numstat=$num"
  if ($num -notmatch '^1\s+1') { throw 'VERIFY_FAIL: semantic edit is still obscured by normalization noise.' }
  Write-Host 'VERIFY_PASS: learner-owned repository policy keeps the diff focused.'
} finally { Pop-Location }