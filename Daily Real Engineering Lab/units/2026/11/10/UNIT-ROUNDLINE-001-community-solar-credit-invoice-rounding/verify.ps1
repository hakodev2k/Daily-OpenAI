$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
  & dotnet run --project starter/SolarCredits.csproj -- --assert-contract
  if ($LASTEXITCODE -ne 0) { throw "Learner implementation violates the contract" }
  Write-Output "VERIFIED: all invoice cases match the contract"
} finally { Pop-Location }
