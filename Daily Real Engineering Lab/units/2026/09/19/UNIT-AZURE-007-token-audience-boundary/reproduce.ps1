$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/AzureIdentityLab.csproj -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
