$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try {
  dotnet build --nologo | Out-Host
  if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
  foreach ($mode in @('honored','ignored')) {
    $output = dotnet run --no-build -- $mode
    $code = $LASTEXITCODE
    $output | Out-Host
    if ($code -ne 0 -or $output -notmatch 'shaMatch=True' -or $output -notmatch 'bytes=128') { throw "Integrity contract failed for $mode" }
  }
  Write-Host 'PASS: both resume modes preserve full-file integrity.'
} finally { Pop-Location }
