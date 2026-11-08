$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/ScholarshipRank.csproj -- --verify
  $code = $LASTEXITCODE
  if ($code -eq 0) { throw 'Expected the original starter to reproduce a failing contract.' }
  Write-Host 'REPRODUCED: original starter violates ranking contract.'
} finally { Pop-Location }
