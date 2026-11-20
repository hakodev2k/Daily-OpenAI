$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project starter/CatalogLab.csproj -- repro
  if ($LASTEXITCODE -eq 0) { throw 'Expected starter symptom but process succeeded' }
  Write-Host 'REPRO PASS: second refresh failed as expected'
} finally { Pop-Location }
