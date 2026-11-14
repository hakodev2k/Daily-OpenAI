$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/GrantReplay.csproj --
  if ($LASTEXITCODE -ne 2) { throw "Expected divergent replay exit code 2; got $LASTEXITCODE" }
  Write-Host 'REPRODUCED: contradictory decisions'
} finally { Pop-Location }
