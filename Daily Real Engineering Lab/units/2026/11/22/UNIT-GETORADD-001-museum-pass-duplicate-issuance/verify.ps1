$ErrorActionPreference = 'Stop'
dotnet build (Join-Path $PSScriptRoot 'starter/PassLab.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
for ($i=0; $i -lt 10; $i++) {
  dotnet run --no-build --project (Join-Path $PSScriptRoot 'starter/PassLab.csproj') -c Release -- --assert
  if ($LASTEXITCODE -ne 0) { throw "Invariant failed on iteration $i" }
}
Write-Host 'PASS: ten concurrent runs preserved single issuance'
