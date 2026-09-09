$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet run --project "$root/starter/TenantRegistryLab.csproj" --configuration Release -- --run
if ($LASTEXITCODE -ne 0) {
    throw "Starter run failed with exit code $LASTEXITCODE."
}
