$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/AzureIdentityLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
