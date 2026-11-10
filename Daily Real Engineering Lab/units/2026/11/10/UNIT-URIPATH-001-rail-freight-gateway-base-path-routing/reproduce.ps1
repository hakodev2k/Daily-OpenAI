$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project starter/RailGateway.csproj -- --reproduce
  if ($LASTEXITCODE -ne 0) { throw "Starter symptom not reproduced (exit $LASTEXITCODE)." }
  Write-Host "Reproduction confirmed; expected URI mismatches observed."
} finally { Pop-Location }
