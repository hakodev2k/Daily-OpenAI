$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot '.work'
Remove-Item $root -Recurse -Force -ErrorAction SilentlyContinue
New-Item $root -ItemType Directory | Out-Null
Push-Location $root
try {
  git init -q
  git config user.email 'lab@example.invalid'
  git config user.name 'Engineering Lab'
  git config core.autocrlf false
  Copy-Item ../starter/appsettings.sample ./appsettings.sample
  git add appsettings.sample
  git commit -q -m baseline

  $text = Get-Content ./appsettings.sample -Raw
  $text = $text -replace "`r?`n", "`r`n"
  $text = $text -replace 'TimeoutSeconds=30', 'TimeoutSeconds=45'
  [IO.File]::WriteAllText((Join-Path $PWD 'appsettings.sample'), $text)

  $changed = git diff --numstat -- appsettings.sample
  git diff --stat -- appsettings.sample
  Write-Host "numstat=$changed"
  if ($changed -notmatch '^5\s+5') { throw 'Expected whole-file review noise was not reproduced.' }
  Write-Host 'REPRODUCED: one semantic edit is hidden inside a whole-file diff.'
} finally { Pop-Location }