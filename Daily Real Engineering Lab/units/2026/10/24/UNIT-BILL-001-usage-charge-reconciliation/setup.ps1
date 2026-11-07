$ErrorActionPreference='Stop'
Push-Location $PSScriptRoot
try {
 dotnet --version
 if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
 dotnet restore starter/BillingReconciliation.Starter.csproj
 if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally { Pop-Location }
