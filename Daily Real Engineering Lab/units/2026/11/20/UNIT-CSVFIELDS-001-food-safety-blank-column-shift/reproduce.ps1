$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot 'starter')
try {
  dotnet run --project FoodLabImport.csproj --no-launch-profile
  $code = $LASTEXITCODE
  if ($code -ne 2) { throw "Expected starter symptom exit 2, received $code" }
  Write-Host 'Reproduction confirmed: at least one input row was rejected or misinterpreted.'
} finally { Pop-Location }
