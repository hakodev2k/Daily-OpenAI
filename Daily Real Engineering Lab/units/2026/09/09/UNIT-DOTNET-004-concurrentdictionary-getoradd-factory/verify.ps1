$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet restore "$root/starter/TenantRegistryLab.csproj"
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed." }

dotnet build "$root/starter/TenantRegistryLab.csproj" --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed." }

dotnet run --project "$root/starter/TenantRegistryLab.csproj" --configuration Release --no-build -- --verify
if ($LASTEXITCODE -ne 0) {
    throw "Verification failed. The learner-editable starter/ path still violates the per-key initialization invariant."
}

Write-Host "Verification passed for learner code in starter/."
