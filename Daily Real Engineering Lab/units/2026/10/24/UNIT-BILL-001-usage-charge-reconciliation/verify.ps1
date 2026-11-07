$ErrorActionPreference='Stop'
Push-Location $PSScriptRoot
try {
 dotnet run --project starter/BillingReconciliation.Starter.csproj -- verify
 if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally { Pop-Location }
