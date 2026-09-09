$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet restore "$root/starter/TenantRegistryLab.csproj"
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed." }

dotnet build "$root/starter/TenantRegistryLab.csproj" --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed." }

dotnet run --project "$root/starter/TenantRegistryLab.csproj" --configuration Release --no-build -- --reproduce
if ($LASTEXITCODE -ne 0) {
    throw "Expected starter symptom was not reproduced. Review the console evidence."
}

Write-Host "Reproduction contract satisfied. Record your hypotheses before changing starter/."
